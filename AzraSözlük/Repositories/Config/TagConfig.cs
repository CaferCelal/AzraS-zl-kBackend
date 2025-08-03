using AzraSözlük.Constants;
using AzraSözlük.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzraSözlük.Repositories.Config;

public class TagConfig: IEntityTypeConfiguration<TagModel>
{
    public void Configure(EntityTypeBuilder<TagModel> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxIdLength);
        
        builder.HasIndex(t => t.Id)
            .IsUnique();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(ProgramConstants.MaxTagLength);
        
        builder.Property(c => c.GeneratedDate)
            .IsRequired()
            .HasColumnType("date");

        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tags)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}