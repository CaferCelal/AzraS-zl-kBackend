using AzraSözlük.Constants;
using AzraSözlük.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzraSözlük.Repositories.Config;

public class CommentConfig:IEntityTypeConfiguration<CommentModel>
{
    public void Configure(EntityTypeBuilder<CommentModel> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);
        
        builder.HasIndex(c => c.Id)
            .IsUnique();
        
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxContentLength);
        
        builder.Property(c => c.GeneratedDate)
            .IsRequired()
            .HasColumnType("date");


        builder.HasOne(c => c.Blog)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}