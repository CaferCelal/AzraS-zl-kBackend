using AzraSözlük.Models;
using AzraSözlük.Models.Dtos;
using AzraSözlük.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AzraSözlük.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Auth controller is working!");
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto dto, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetService<UserManager<UserModel>>();
            var passwordHasher = userManager.PasswordHasher;

            if (userManager == null || passwordHasher == null)
            {
                return BadRequest(new { message = "UserManager or PasswordHasher service not found." });
            }
            
            UserModel user = new UserModel
            {
                UserName = dto.UserName,
                Email = dto.UserEmail,
            };
            passwordHasher.HashPassword(user,dto.Password);
            var result = userManager.CreateAsync(user, dto.Password).Result;
            if (result.Succeeded)
            {
                return Ok(new { message = "User created successfully!" });
            }
            else
            {
                return BadRequest(new { message = "User creation failed!", errors = result.Errors });
            }
        }
        
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers([FromServices] IServiceProvider sp)
        {
            var dbContext = sp.GetService<RepositoryContext>();
            
            if (dbContext == null)
            {
                return NotFound(new { message = "Database context not found." });
            }
            var users = dbContext.Users.ToList();
            
            if (users == null || !users.Any())
            {
                return NotFound(new { message = "No users found." });
            }
            
            return Ok(users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.PhoneNumber,
                u.EmailConfirmed,
                u.PhoneNumberConfirmed,
                u.LockoutEnabled,
                u.TwoFactorEnabled
            }));
        }
    }
}
