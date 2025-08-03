using System.Security.Claims;
using AzraSözlük.Constants;
using AzraSözlük.Data;
using AzraSözlük.Data.Dtos.Comment;
using AzraSözlük.Data.Models;
using AzraSözlük.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzraSözlük.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class Comment : ControllerBase
    {
        [HttpGet("get/{blogId}")]
        public async Task<IResult> GetComments([FromRoute] string blogId, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var dbContext = sp.GetRequiredService<RepositoryContext>();
            
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                        data: "User not authenticated.");
            }

            var comments = await dbContext.Comments
                .Where(c => c.UserId == user.Id && c.BlogId == blogId)
                .Include(c => c.User)
                .Include(c => c.Blog)
                .Select(c => new GetCommentDto(
                    c.Id,
                    c.User.UserName!,
                    c.Content,
                    c.GeneratedDate,
                    c.UserCommentLikes.Count(c => c.Like == true),
                    c.UserCommentLikes.Count(c => c.Dislike == true),
                    c.UserCommentLikes.Any(c => c.UserId == user.Id && c.Like == true),
                    c.UserCommentLikes.Any(c => c.UserId == user.Id && c.Dislike == true)))
                .ToListAsync();

            return TypedResults.Json(statusCode: StatusCodes.Status200OK, data: comments);
        }

        [HttpPost("create/{blogId}")]
        public async Task<IResult> CreateComment([FromRoute] string blogId,
            [FromBody] CreateCommentDto dto, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var dbContext = sp.GetRequiredService<RepositoryContext>();
            
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }
            
            if (string.IsNullOrWhiteSpace(dto.CommentContent))
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: "Comment content cannot be empty.");
            }
            
            if (dto.CommentContent.Length > ProgramConstants.MaxContentLength)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: $"Comment content cannot be longer than {ProgramConstants.MaxContentLength} characters.");
            }

            await dbContext.Comments.AddAsync(new CommentModel(blogId, user.Id, dto.CommentContent));
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status503ServiceUnavailable,
                    data: ProgramConstants.UnexpectedError);
            }
            return TypedResults.NoContent();
        }
        
        
        
        [HttpPost("like/{commentId}")]
        public async Task<IResult> LikeComment([FromRoute] string commentId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Like, commentId, sp, HttpContext.User);
        }

        [HttpPost("unlike/{commentId}")]
        public async Task<IResult> UnlikeComment([FromRoute] string commentId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Unlike, commentId, sp, HttpContext.User);
        }
        
        [HttpPost("dislike/{commentId}")]
        public async Task<IResult> DislikeComment([FromRoute] string commentId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Dislike, commentId, sp, HttpContext.User);
        }
        
        [HttpPost("undislike/{commentId}")]
        public async Task<IResult> UndislikeComment([FromRoute] string commentId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Undislike, commentId, sp, HttpContext.User);
        }
        
        private async Task<IResult> LikeManager(LikeSituation situation, string commentId,
            IServiceProvider sp, ClaimsPrincipal userClaim)
        {
            var dbContext = sp.GetRequiredService<RepositoryContext>();
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            
            var user = await userManager.GetUserAsync(userClaim);
            if (user is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }
            
            var likeBlogElement = dbContext.UserCommentLikes
                .FirstOrDefault(ucl => ucl.UserId == user.Id && ucl.CommentId == commentId);

            if (likeBlogElement is null)
            {
                await dbContext.UserCommentLikes.AddAsync(new UserCommentLikeModel(user.Id, commentId));
                await dbContext.SaveChangesAsync();
            }

            int result = 0;
            
            switch (situation)
            {
                case LikeSituation.Like:
                {
                    result = await dbContext.UserCommentLikes
                        .Where(ucl => ucl.UserId == user.Id && ucl.CommentId == commentId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ucl => ucl.Like, true)
                            .SetProperty(ucl => ucl.Dislike, false)
                            .SetProperty(ucl => ucl.Neutral, false)
                        );
                    break;
                }
                case LikeSituation.Unlike:
                {
                    result = await dbContext.UserCommentLikes
                        .Where(ucl => ucl.UserId == user.Id && ucl.CommentId == commentId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ucl => ucl.Like, false)
                            .SetProperty(ucl => ucl.Dislike, false)
                            .SetProperty(ucl => ucl.Neutral, true)
                        );
                    break;
                }
                case LikeSituation.Dislike:
                {
                    result = await dbContext.UserCommentLikes
                        .Where(ucl => ucl.UserId == user.Id && ucl.CommentId == commentId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ucl => ucl.Like, false)
                            .SetProperty(ucl => ucl.Dislike, true)
                            .SetProperty(ucl => ucl.Neutral, false)
                        );
                    break;
                }
                case LikeSituation.Undislike:
                {
                    result = await dbContext.UserCommentLikes
                        .Where(ucl => ucl.UserId == user.Id && ucl.CommentId == commentId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ucl => ucl.Like, false)
                            .SetProperty(ucl => ucl.Dislike, false)
                            .SetProperty(ucl => ucl.Neutral, true)
                        );
                    break;
                }
            }
            
            if (result > 0)
            {
                return TypedResults.Ok("Like updated successfully.");
            }
            else
            {
                return TypedResults.Json(statusCode: StatusCodes.Status404NotFound ,data:"Like record not found.");
            }
        }
    }
}
