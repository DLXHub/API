using API.Features.Pages.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Features.Pages.Configuration;

/// <summary>
/// Configuration class for the Page entity.
/// </summary>
public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    /// <summary>
    /// Configures the Page entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.SeoTitle)
            .HasMaxLength(255);

        builder.Property(p => p.MetaDescription)
            .HasMaxLength(500);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.LinkTarget)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Components)
            .IsRequired()
            .HasColumnType("jsonb"); // Using PostgreSQL's native JSON type

        builder.Property(p => p.Version)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.Slug)
            .IsUnique()
            .HasFilter(null)
            .HasDatabaseName("IX_Pages_Slug_NonDeleted");

        builder.HasIndex(p => p.LinkTarget)
            .IsUnique()
            .HasFilter(null)
            .HasDatabaseName("IX_Pages_LinkTarget_NonDeleted");

        builder.HasIndex(p => p.Status);

        // Relationships
        builder.HasOne(p => p.OriginalPage)
            .WithMany(p => p.DraftVersions)
            .HasForeignKey(p => p.OriginalPageId)
            .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete drafts when original is deleted

        builder.HasOne(p => p.PublishedBy)
            .WithMany()
            .HasForeignKey(p => p.PublishedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Ensure only one published version exists per original page
        builder.HasIndex(p => new { p.OriginalPageId, p.IsPublished })
            .HasFilter(null)
            .HasDatabaseName("IX_Pages_OriginalPageId_IsPublished_Unique")
            .IsUnique();
    }
}