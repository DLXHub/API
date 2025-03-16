using API.Features.Downloads.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Features.Downloads.Configuration;

/// <summary>
/// Configuration class for the Download entity.
/// </summary>
public class DownloadConfiguration : IEntityTypeConfiguration<Download>
{
    /// <summary>
    /// Configures the Download entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Download> builder)
    {
        // Primary key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Language)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Quality)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.MediaType)
            .IsRequired();

        // Indexes for filtering
        builder.HasIndex(d => d.Language);
        builder.HasIndex(d => d.Quality);
        builder.HasIndex(d => d.MediaType);

        // Composite indexes for efficient querying
        builder.HasIndex(d => new { d.MovieId, d.Language, d.Quality })
            .HasFilter("movie_id IS NOT NULL");
        builder.HasIndex(d => new { d.TvShowId, d.Language, d.Quality })
            .HasFilter("tv_show_id IS NOT NULL");
        builder.HasIndex(d => new { d.SeasonId, d.Language, d.Quality })
            .HasFilter("season_id IS NOT NULL");
        builder.HasIndex(d => new { d.EpisodeId, d.Language, d.Quality })
            .HasFilter("episode_id IS NOT NULL");

        // Relationships
        builder.HasOne(d => d.Movie)
            .WithMany()
            .HasForeignKey(d => d.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.TvShow)
            .WithMany()
            .HasForeignKey(d => d.TvShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Season)
            .WithMany()
            .HasForeignKey(d => d.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Episode)
            .WithMany()
            .HasForeignKey(d => d.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Check constraint to ensure only one media type is set
        builder.ToTable(t => t.HasCheckConstraint("CK_Download_SingleMediaType",
            "((CASE WHEN movie_id IS NOT NULL THEN 1 ELSE 0 END) + " +
            "(CASE WHEN tv_show_id IS NOT NULL THEN 1 ELSE 0 END) + " +
            "(CASE WHEN season_id IS NOT NULL THEN 1 ELSE 0 END) + " +
            "(CASE WHEN episode_id IS NOT NULL THEN 1 ELSE 0 END)) = 1"));
    }
}