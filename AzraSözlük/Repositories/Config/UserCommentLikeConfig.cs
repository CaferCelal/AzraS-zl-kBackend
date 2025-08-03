using AzraSözlük.Constants;
using AzraSözlük.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzraSözlük.Repositories.Config;

public class UserCommentLikeConfig : IEntityTypeConfiguration<UserCommentLikeModel>
{
    public void Configure(EntityTypeBuilder<UserCommentLikeModel> builder)
    {
        builder.ToTable("UserCommentLikes");

        builder.HasKey(ucl => ucl.Id);

        builder.Property(ucl => ucl.Id)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);

        builder.HasIndex(ucl => ucl.Id)
            .IsUnique();

        builder.Property(ucl => ucl.UserId)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);

        builder.Property(ucl => ucl.CommentId)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);

        builder.Property(ucl => ucl.Like)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ucl => ucl.Dislike)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ucl => ucl.Neutral)
            .IsRequired()
            .HasDefaultValue(true);

        // 🔗 Relationship with User
        builder.HasOne(ucl => ucl.User)
            .WithMany(u => u.UserCommentLikes)
            .HasForeignKey(ucl => ucl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔗 Relationship with Comment
        builder.HasOne(ucl => ucl.Comment)
            .WithMany(c => c.UserCommentLikes)
            .HasForeignKey(ucl => ucl.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}