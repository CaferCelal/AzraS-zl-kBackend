using AzraSözlük.Constants;
using AzraSözlük.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzraSözlük.Repositories.Config;

public class UserBlogLikeConfig : IEntityTypeConfiguration<UserBlogLikeModel>
{
    public void Configure(EntityTypeBuilder<UserBlogLikeModel> builder)
    {
        builder.ToTable("UserBlogLikes");

        builder.HasKey(ubl => ubl.Id);

        builder.Property(ubl => ubl.Id)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);

        builder.HasIndex(ubl => ubl.Id)
            .IsUnique();

        builder.Property(ubl => ubl.UserId)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);

        builder.Property(ubl => ubl.BlogId)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);

        builder.Property(ubl => ubl.Like)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ubl => ubl.Dislike)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ubl => ubl.Neutral)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(ubl => ubl.User)
            .WithMany(u => u.UserBlogLikes)
            .HasForeignKey(ubl => ubl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ubl => ubl.Blog)
            .WithMany(b => b.UserBlogLikes)
            .HasForeignKey(ubl => ubl.BlogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}