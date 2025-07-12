using AzraSözlük.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AzraSözlük.Repositories;

public class RepositoryContext : IdentityDbContext<UserModel>
{
    
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new UserConfig());
    }
}