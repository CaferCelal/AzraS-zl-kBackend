using System.Globalization;
using AzraSözlük.Constants;
using AzraSözlük.Data.Dtos;
using AzraSözlük.Data.Models;
using AzraSözlük.Repositories;
using CaferEmailLib;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzraSözlük.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class Auth : ControllerBase
    {

        [HttpGet("me")]
        public async Task<IResult> Me([FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userStatus = new
            {
                IsLoggedIn = user is not null
            };
            return TypedResults.Json(userStatus);
        }


        [HttpPost("sign-up")]
        public async Task<IResult> SignUp([FromBody] SignUpDto dto, [FromServices] IServiceProvider sp)
        {
            var signInManager = sp.GetRequiredService<SignInManager<UserModel>>();
            var isValidDto = dto.IsValid();
            
            if (isValidDto.isValid == false)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: new {message = isValidDto.errors});
            }
            
            var existingUser = await signInManager.UserManager.FindByEmailAsync(dto.UserEmail);
            
            if (existingUser != null)
            {
                return TypedResults.Json(
                    statusCode: StatusCodes.Status409Conflict,
                    data: new { message = "Sorry, email is already in use." }
                );
            }
            
            var passwordHasher = signInManager.UserManager.PasswordHasher;
            
            
            UserModel user = new UserModel
            {
                UserName = dto.UserName,
                Email = dto.UserEmail,
            };
            passwordHasher.HashPassword(user,dto.UserPassword);
            var result = signInManager.UserManager.CreateAsync(user, dto.UserPassword).Result;
            
            if (result.Succeeded)
            {
                var emailSender = sp.GetRequiredService<ICaferEmailSender>();
                
                var email = emailSender.BuildEmail(user.Email, user.UserName, "Welcome to AzraSözlük!",
                    "Thank you for signing up! Your account has been created successfully.");
                
                var emailResult = await emailSender.SendEmailAsync(email, SecureSocketOptions.StartTls);
                
                if (emailResult is not null)
                {
                    throw emailResult;
                }
                
                await signInManager.SignInAsync(user, isPersistent: false);
                
                return TypedResults.NoContent();
            }
            else
            {
                return TypedResults.Json(
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    data: new { message = ProgramConstants.UnexpectedError }
                );
            }
        }
        
        [HttpPost("sign-in")]
        public async Task<IResult> SignIn([FromBody] SignInDto dto, [FromServices] IServiceProvider sp)
        {
            var signInManager = sp.GetRequiredService <SignInManager<UserModel>>();
            
            var isValidDto = dto.IsValid();
            if (isValidDto.isValid == false)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status400BadRequest,
                    data: isValidDto.errors);
            }
            var user = await signInManager.UserManager.FindByEmailAsync(dto.UserEmail);

            var invalidUserError = TypedResults.Json(
                statusCode: StatusCodes.Status401Unauthorized,
                data: "Invalid email or password.");
            
            if (user == null)
            {
                return invalidUserError;
            }
            
            var result = await signInManager.CheckPasswordSignInAsync(user, dto.UserPassword, false);

            if (!result.Succeeded)
            {
                return invalidUserError;
            }
            
            await signInManager.SignInAsync(user, false);

            return TypedResults.Ok();
        }
        
        [HttpPost("sign-out")]
        public async Task<IResult> SignOut([FromServices] IServiceProvider sp)
        {
            var signInManager = sp.GetRequiredService<SignInManager<UserModel>>();
            await signInManager.SignOutAsync();
            return TypedResults.Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IResult> ForgotPassword(
            [FromBody] ForgotPasswordDto dto, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            var emailSender = sp.GetRequiredService<ICaferEmailSender>();

            
            var user = await userManager.FindByEmailAsync(dto.UserEmail);
            
            var isValidDto = dto.IsValid();

            if (isValidDto.isValid == false)
            {
                return TypedResults.Json(
                    statusCode: StatusCodes.Status400BadRequest,
                    data: isValidDto.errors);
            }

            if (user == null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status404NotFound, data:$"No user found with email: {dto.UserEmail}");
            }

            string tokenName = "ForgotPassword";
            string tokenExpiry = "ForgotPasswordExpiry";
            var randomUser = new UserModel(); // To avoid same code generation
            
            var resetPasswordCode = await userManager.GenerateTwoFactorTokenAsync(randomUser, TokenOptions.DefaultEmailProvider);
            var resetPasswordCodeExpiry = DateTime.UtcNow.AddMinutes(10); // Code valid for 10 minutes

            await userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultEmailProvider, tokenName, resetPasswordCode);
            await userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultEmailProvider, tokenExpiry, resetPasswordCodeExpiry.ToString("o"));
            
           
            var email = emailSender.BuildEmail(dto.UserEmail, user.UserName, "Password Reset Request",
                $"To reset your password, please use the following token: {resetPasswordCode}");

            await emailSender.SendEmailAsync(email, SecureSocketOptions.StartTls);

            return TypedResults.Ok();
        }

        [HttpPost("verify-token")]
        public async Task<IResult> VerifyToken([FromBody] VerifyTokenDto dto, [FromServices] IServiceProvider sp)
        {
            var dbContext = sp.GetRequiredService<RepositoryContext>();
            var signInManager = sp.GetRequiredService<SignInManager<UserModel>>();
            var user = await signInManager.UserManager.FindByEmailAsync(dto.UserEmail);
            
            if (user == null)
            {
                return TypedResults.Json(
                    statusCode: StatusCodes.Status404NotFound,
                    data: "User not found.");
            }
            
            var otpCodeStored = await dbContext.UserTokens
                .Where(t => t.UserId == user.Id &&
                            t.LoginProvider == TokenOptions.DefaultEmailProvider &&
                            t.Name == "ForgotPassword").FirstOrDefaultAsync();
                
            var otpExpiryStored = await dbContext.UserTokens.
                Where(t => t.UserId == user.Id && 
                           t.LoginProvider == TokenOptions.DefaultEmailProvider && 
                           t.Name == "ForgotPasswordExpiry").FirstOrDefaultAsync();
            
            if (string.IsNullOrWhiteSpace(otpCodeStored?.Value) || string.IsNullOrWhiteSpace(otpExpiryStored?.Value))
            {
                return TypedResults.Json(statusCode: StatusCodes.Status404NotFound,
                    data: "Reset password code not found, please request a new one.");
                
            }
            
            var isValidOtp = dto.UserToken == otpCodeStored.Value;
            
            if (!isValidOtp)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "Invalid reset password code.");
            }

            var otpExpiry = DateTime.Parse(otpExpiryStored.Value, null, DateTimeStyles.RoundtripKind);
            if (DateTime.UtcNow > otpExpiry)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status410Gone,
                    data: "Reset password code expired");
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            return TypedResults.Ok();  
        }

        [HttpPost("reset-password")]
        public async Task<IResult> ResetPassword([FromBody] ResetPasswordDto dto, [FromServices] IServiceProvider sp)
        {
            var dbContext = sp.GetRequiredService<RepositoryContext>();
            var userManager = sp.GetRequiredService<UserManager<UserModel>>();
            
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return TypedResults.Json(statusCode: StatusCodes.Status401Unauthorized,
                    data: "User not authenticated.");
            }
            
            string newPassword = userManager.PasswordHasher.HashPassword(user, dto.NewPassword);
            
            await dbContext.Users
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.PasswordHash, newPassword));

            return TypedResults.Ok();
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
