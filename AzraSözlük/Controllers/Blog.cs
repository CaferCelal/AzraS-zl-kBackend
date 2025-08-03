using AzraSözlük.Constants;
using AzraSözlük.Data;
using AzraSözlük.Data.Dtos;
using AzraSözlük.Data.Dtos.Blog;
using AzraSözlük.Data.Models;
using AzraSözlük.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzraSözlük.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class Blog : ControllerBase
    {

        private async Task<UserModel?> GetUserAsync(UserManager<UserModel> userManager)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user is not null)
            {
                return user;
            }

            return null;
        }

        [HttpPost("create/{tagId}")]
        public async Task<IResult> CreateBlog([FromRoute] string? tagId,
            [FromBody] CreateBlogDto dto, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var dbContext = sp.GetRequiredService<RepositoryContext>();

            var user = await GetUserAsync(userManager);
            if (user is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }

            var tag = await dbContext.Tags.FindAsync(tagId);

            if (tag is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status404NotFound,
                    data: "Tag not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.BlogContent) ||
                string.IsNullOrWhiteSpace(dto.BlogHeader))
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: "Blog content and header cannot be empty.");
            }

            if (dto.BlogContent.Length > ProgramConstants.MaxContentLength ||
                dto.BlogHeader.Length > ProgramConstants.MaxBlogHeaderLength)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: $"Blog content cannot be longer than {ProgramConstants.MaxContentLength} characters " +
                          $"and header cannot be longer than {ProgramConstants.MaxBlogHeaderLength} characters.");
            }

            BlogModel blog = new(dto.BlogHeader, dto.BlogContent, user.Id, tagId);
            await dbContext.Blogs.AddAsync(blog);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status503ServiceUnavailable,
                    data: ProgramConstants.UnexpectedError);
            }

            return TypedResults.NoContent();
        }

        [HttpPatch("update/{blogId}")]
        public async Task<IResult> UpdateBlog([FromRoute] string blogId,
            [FromBody] CreateBlogDto dto, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var dbContext = sp.GetRequiredService<RepositoryContext>();

            var user = await GetUserAsync(userManager);
            if (user is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }

            var blog = await dbContext.Blogs.FindAsync(blogId);

            if (blog is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status404NotFound,
                    data: "Blog not found.");
            }

            if (blog.UserId != user.Id)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status403Forbidden,
                    data: "You are not the owner of this blog.");
            }

            if (string.IsNullOrWhiteSpace(dto.BlogContent) ||
                string.IsNullOrWhiteSpace(dto.BlogHeader))
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: "Blog content and header cannot be empty.");
            }

            if (dto.BlogContent.Length > ProgramConstants.MaxContentLength ||
                dto.BlogHeader.Length > ProgramConstants.MaxBlogHeaderLength)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: $"Blog content cannot be longer than {ProgramConstants.MaxContentLength} characters " +
                          $"and header cannot be longer than {ProgramConstants.MaxBlogHeaderLength} characters.");
            }

            blog.BlogHeader = dto.BlogHeader;
            blog.BlogContent = dto.BlogContent;
            blog.LastUpdateDate = DateTime.Now;

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

        [HttpGet("get/{tagId}")]
        public async Task<IResult> GetBlogs([FromRoute] string? tagId, [FromServices] IServiceProvider sp)
        {
            var dbContext = sp.GetRequiredService<RepositoryContext>();
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            
            var user = await GetUserAsync(userManager);
            if (user is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }
            
            if (string.IsNullOrWhiteSpace(tagId))
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: "Tag ID cannot be empty.");
            }

            var tag = await dbContext.Tags
                .Where(t => t.Id == tagId)
                .Include(t => t.User)
                .Select(t => new DetailedTagDto(t.Id, t.Name, t.User.UserName ?? "Unknown User", 
                    t.GeneratedDate))
                .FirstOrDefaultAsync();

            var blogs = await dbContext.Blogs
                .Where(b => b.TagId == tagId)
                .Include(b => b.User)
                .Include(b => b.Tag)
                .Include(b => b.Comments)
                .Include(b => b.UserBlogLikes)
                .OrderByDescending(b => b.LastUpdateDate)
                .Select(b => new GetBlogDto(
                    b.Id,
                    b.User.UserName ?? "Unknown User",
                    b.BlogHeader,
                    b.BlogContent,
                    b.GeneratedDate,
                    b.LastUpdateDate,
                    b.UserBlogLikes.Count(ubl => ubl.Like == true),
                    b.UserBlogLikes.Count(ubl => ubl.Dislike == true),
                    b.Comments.Count,
                    b.UserBlogLikes.Any(ubl => ubl.UserId == user.Id && ubl.Like == true),
                    b.UserBlogLikes.Any(ubl => ubl.UserId == user.Id && ubl.Dislike == true)))
                        .ToListAsync();

            var returnData = new
            {
                Tag = tag,
                Blogs = blogs
            };

            return TypedResults.Json(statusCode: StatusCodes.Status200OK, data: returnData);
        }

        [HttpPost("like/{blogId}")]
        public async Task<IResult> LikeBlog([FromRoute] string blogId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Like, blogId, sp);
        }

        [HttpPost("unlike/{blogId}")]
        public async Task<IResult> UnlikeBlog([FromRoute] string blogId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Unlike, blogId, sp);
        }
        
        [HttpPost("dislike/{blogId}")]
        public async Task<IResult> DislikeBlog([FromRoute] string blogId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Dislike, blogId, sp);
        }
        
        [HttpPost("undislike/{blogId}")]
        public async Task<IResult> UndislikeBlog([FromRoute] string blogId, [FromServices] IServiceProvider sp)
        {
            return await LikeManager(LikeSituation.Undislike, blogId, sp);
        }


        private async Task<IResult> LikeManager(LikeSituation situation, string blogId, IServiceProvider sp)
        {
            var dbContext = sp.GetRequiredService<RepositoryContext>();
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            
            var user = await GetUserAsync(userManager);
            if (user is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }
            
            var likeBlogElement = dbContext.UserBlogLikes
                .FirstOrDefault(ubl => ubl.UserId == user.Id && ubl.BlogId == blogId);

            if (likeBlogElement is null)
            {
                await dbContext.UserBlogLikes.AddAsync(new UserBlogLikeModel(user.Id, blogId));
                await dbContext.SaveChangesAsync();
            }

            int result = 0;
            
            switch (situation)
            {
                case LikeSituation.Like:
                {
                    result = await dbContext.UserBlogLikes
                        .Where(ubl => ubl.UserId == user.Id && ubl.BlogId == blogId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ubl => ubl.Like, true)
                            .SetProperty(ubl => ubl.Dislike, false)
                            .SetProperty(ubl => ubl.Neutral, false)
                        );
                    break;
                }
                case LikeSituation.Unlike:
                {
                    result = await dbContext.UserBlogLikes
                        .Where(ubl => ubl.UserId == user.Id && ubl.BlogId == blogId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ubl => ubl.Like, false)
                            .SetProperty(ubl => ubl.Dislike, false)
                            .SetProperty(ubl => ubl.Neutral, true)
                        );
                    break;
                }
                case LikeSituation.Dislike:
                {
                    result = await dbContext.UserBlogLikes
                        .Where(ubl => ubl.UserId == user.Id && ubl.BlogId == blogId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ubl => ubl.Like, false)
                            .SetProperty(ubl => ubl.Dislike, true)
                            .SetProperty(ubl => ubl.Neutral, false)
                        );
                    break;
                }
                case LikeSituation.Undislike:
                {
                    result = await dbContext.UserBlogLikes
                        .Where(ubl => ubl.UserId == user.Id && ubl.BlogId == blogId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(ubl => ubl.Like, false)
                            .SetProperty(ubl => ubl.Dislike, false)
                            .SetProperty(ubl => ubl.Neutral, true)
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
