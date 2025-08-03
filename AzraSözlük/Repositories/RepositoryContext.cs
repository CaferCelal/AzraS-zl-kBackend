using AzraSözlük.Data.Models;
using AzraSözlük.Repositories.Config;
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
        modelBuilder.ApplyConfiguration(new TagConfig());
        modelBuilder.ApplyConfiguration(new BlogConfig());
        modelBuilder.ApplyConfiguration(new CommentConfig());
        modelBuilder.ApplyConfiguration(new UserBlogLikeConfig());
        modelBuilder.ApplyConfiguration(new UserCommentLikeConfig());
    }

    public DbSet<TagModel> Tags { get; set; } = null!;
    public DbSet<BlogModel> Blogs { get; set; } = null!;
    public DbSet<CommentModel> Comments { get; set; } = null!;
    public DbSet<UserCommentLikeModel> UserCommentLikes { get; set; } = null!;
    public DbSet<UserBlogLikeModel> UserBlogLikes { get; set; } = null!;
}