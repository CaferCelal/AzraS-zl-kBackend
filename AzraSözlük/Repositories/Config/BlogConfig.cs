using AzraSözlük.Constants;
using AzraSözlük.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzraSözlük.Repositories.Config;

public class BlogConfig:IEntityTypeConfiguration<BlogModel>
{
    public void Configure(EntityTypeBuilder<BlogModel> builder)
    {
        builder.ToTable("Blogs");

        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);
        
        builder.HasIndex(b => b.Id)
            .IsUnique();

        builder.Property(b => b.BlogHeader)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxBlogHeaderLength);

        builder.Property(b => b.BlogContent)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxContentLength);
        
        builder.Property(b => b.GeneratedDate)
            .IsRequired()
            .HasColumnType("date");

        builder.HasOne(b => b.User)
            .WithMany(u => u.Blogs)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}