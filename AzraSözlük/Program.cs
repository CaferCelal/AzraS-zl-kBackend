using AzraSözlük.Data.Models;
using AzraSözlük.Repositories;
using CaferEmailLib;
using MailKit.Security;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ICaferEmailSender>(sp =>
{
    var config = builder.Configuration;
    return new CaferEmailSender(
        config["Email:Address"]!,
        config["Email:Password"]!,
        config["Email:Issuer"]!,
        config["Email:Smtp"]!,
        int.Parse(config["Email:Port"]!)
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("General", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Name = "AzraSozlukCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
    {
        options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultEmailProvider;
        options.User.RequireUniqueEmail = true;

        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<RepositoryContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<RepositoryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        Exception nullException = new Exception("An unexpected error occurred.");
        var exception = exceptionHandlerPathFeature?.Error ?? nullException;
        var emailSender = context.RequestServices.GetRequiredService<ICaferEmailSender>();
        
        var email = emailSender.BuildEmail("celal.evrenuz@stu.aydin.edu.tr",null,
            "Exception Occured",exception.Message +"\n" + exception.InnerException?.Message);
        await emailSender.SendEmailAsync(email, SecureSocketOptions.StartTls);
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            detail = "An unexpected error occurred."
        });
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("General");

app.MapControllers();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
