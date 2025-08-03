using AzraSözlük.Constants;
using AzraSözlük.Data.Dtos;
using AzraSözlük.Data.Models;
using AzraSözlük.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzraSözlük.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class Tag : ControllerBase
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
        
        [HttpPost("create")]
        public async Task<IResult> CreateTag([FromBody] CreateTagDto dto,
            [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var dbContext = sp.GetRequiredService<RepositoryContext>();

            var user = await GetUserAsync(userManager);
            if (user is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: "Tag name cannot be empty.");
            }

            if (dto.Name.Length > ProgramConstants.MaxTagLength)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: $"Tag name cannot be longer than {ProgramConstants.MaxTagLength} characters.");
            }
            
            if (await dbContext.Tags.AnyAsync(t => t.Name == dto.Name))
            {
                return TypedResults.Json(statusCode: StatusCodes.Status409Conflict,
                    data: "Tag with this name already exists.");
            }
            
            
            TagModel tag = new(dto.Name, user.Id);
            await dbContext.Tags.AddAsync(tag);
            
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


        [HttpGet("get")]
        public async Task<IResult> GetTags([FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var dbContext = sp.GetRequiredService<RepositoryContext>();

            var user = await GetUserAsync(userManager);
            if (user is null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }

            List<GetTagDto> tags;

            try
            {
                tags = await dbContext.Tags
                    .Include(t => t.Blogs)
                    .OrderByDescending(t => t.GeneratedDate)
                    .Select(t => new GetTagDto(t.Id, t.Name, t.GeneratedDate, t.Blogs.Count))
                    .ToListAsync();
            }
            catch (Exception)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status503ServiceUnavailable,
                    data: "An unexpected error occured, please try again later.");
            }
            
            if (!tags.Any())
            {
                return TypedResults.Json(statusCode: StatusCodes.Status404NotFound,
                    data: "No tags found");
            }

            return TypedResults.Json(statusCode: StatusCodes.Status200OK, data: tags);
        }
    }
}
