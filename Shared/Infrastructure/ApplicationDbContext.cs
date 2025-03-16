using System;
using System.Linq.Expressions;
using API.Features.Analytics.Models;
using API.Features.Collections.Models;
using API.Features.Downloads.Configuration;
using API.Features.Downloads.Models;
using API.Features.FeatureFlags.Models;
using API.Features.Genres.Models;
using API.Features.Jobs.Models;
using API.Features.Languages.Models;
using API.Features.Movies.Models;
using API.Features.Pages.Configuration;
using API.Features.Pages.Models;
using API.Features.People.Models;
using API.Features.TvShows.Models;
using API.Shared.Extensions;
using API.Shared.Models;
using Features.Analytics.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Shared.Infrastructure;

/// <summary>
/// The main database context for the application.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

  /// <summary>
  /// Initializes a new instance of the ApplicationDbContext class.
  /// </summary>
  /// <param name="options">The options for this context.</param>
  /// <param name="auditableEntitySaveChangesInterceptor">The interceptor for handling auditable entity changes.</param>
  public ApplicationDbContext(
      DbContextOptions<ApplicationDbContext> options,
      AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
      : base(options)
  {
    _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
  }

  /// <summary>
  /// Gets or sets the PageViews DbSet.
  /// </summary>
  public DbSet<PageView> PageViews => Set<PageView>();

  /// <summary>
  /// Gets or sets the AbTests DbSet.
  /// </summary>
  public DbSet<AbTest> AbTests => Set<AbTest>();

  /// <summary>
  /// Gets or sets the movies DbSet.
  /// </summary>
  public DbSet<Movie> Movies => Set<Movie>();

  /// <summary>
  /// Gets or sets the TV shows DbSet.
  /// </summary>
  public DbSet<TvShow> TvShows => Set<TvShow>();

  /// <summary>
  /// Gets or sets the seasons DbSet.
  /// </summary>
  public DbSet<Season> Seasons => Set<Season>();

  /// <summary>
  /// Gets or sets the episodes DbSet.
  /// </summary>
  public DbSet<Episode> Episodes => Set<Episode>();

  /// <summary>
  /// Gets or sets the people DbSet.
  /// </summary>
  public DbSet<Person> People => Set<Person>();

  /// <summary>
  /// Gets or sets the movie cast DbSet.
  /// </summary>
  public DbSet<MovieCast> MovieCast => Set<MovieCast>();

  /// <summary>
  /// Gets or sets the movie crew DbSet.
  /// </summary>
  public DbSet<MovieCrew> MovieCrew => Set<MovieCrew>();

  /// <summary>
  /// Gets or sets the TV show cast DbSet.
  /// </summary>
  public DbSet<TvShowCast> TvShowCast => Set<TvShowCast>();

  /// <summary>
  /// Gets or sets the TV show crew DbSet.
  /// </summary>
  public DbSet<TvShowCrew> TvShowCrew => Set<TvShowCrew>();

  /// <summary>
  /// Gets or sets the collections DbSet.
  /// </summary>
  public DbSet<Collection> Collections => Set<Collection>();

  /// <summary>
  /// Gets or sets the movie collections DbSet.
  /// </summary>
  public DbSet<MovieCollection> MovieCollections => Set<MovieCollection>();

  /// <summary>
  /// Gets or sets the downloads DbSet.
  /// </summary>
  public DbSet<Download> Downloads => Set<Download>();

  public DbSet<Genre> Genres => Set<Genre>();
  public DbSet<API.Features.Genres.Models.MediaGenre> MediaGenres => Set<API.Features.Genres.Models.MediaGenre>();

  /// <summary>
  /// Gets or sets the pages DbSet.
  /// </summary>
  public DbSet<Page> Pages => Set<Page>();

  /// <summary>
  /// Gets or sets the Jobs DbSet.
  /// </summary>
  public DbSet<Job> Jobs => Set<Job>();

  /// <summary>
  /// Gets or sets the FeatureFlags DbSet.
  /// </summary>
  public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

  /// <summary>
  /// Gets or sets the Languages DbSet.
  /// </summary>
  public DbSet<Language> Languages => Set<Language>();

  /// <summary>
  /// Gets or sets the PerformanceMetrics DbSet.
  /// </summary>
  public DbSet<PerformanceMetric> PerformanceMetrics => Set<PerformanceMetric>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Convert all table names to snake_case
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
      // Skip Identity tables from renaming if you want to keep their default names
      if (entity.ClrType.Namespace != typeof(IdentityRole).Namespace)
      {
        entity.SetTableName(entity.GetTableName()?.ToSnakeCase());

        foreach (var property in entity.GetProperties())
        {
          property.SetColumnName(property.GetColumnName().ToSnakeCase());
        }
      }
    }

    // Configure relationships for audit properties
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
      {
        modelBuilder.Entity(entityType.ClrType, builder =>
        {
          // CreatedBy relationship
          builder.HasOne(typeof(ApplicationUser), "CreatedBy")
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);

          // ModifiedBy relationship
          builder.HasOne(typeof(ApplicationUser), "ModifiedBy")
            .WithMany()
            .HasForeignKey("ModifiedById")
            .OnDelete(DeleteBehavior.Restrict);

          // DeletedBy relationship
          builder.HasOne(typeof(ApplicationUser), "DeletedBy")
            .WithMany()
            .HasForeignKey("DeletedById")
            .OnDelete(DeleteBehavior.Restrict);
        });
      }
    }

    // Configure global query filter for soft delete
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
      {
        var parameter = Expression.Parameter(entityType.ClrType, "e");
        var property = Expression.Property(parameter, "IsDeleted");
        var constant = Expression.Constant(false);
        var filter = Expression.Lambda(Expression.Equal(property, constant), parameter);

        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);

        // Add the same filter for any required relationships
        foreach (var navigation in entityType.GetNavigations())
        {
          if (navigation.IsCollection && typeof(BaseEntity).IsAssignableFrom(navigation.TargetEntityType.ClrType))
          {
            var targetParam = Expression.Parameter(navigation.TargetEntityType.ClrType, "t");
            var targetProperty = Expression.Property(targetParam, "IsDeleted");
            var targetFilter = Expression.Lambda(Expression.Equal(targetProperty, constant), targetParam);

            modelBuilder.Entity(navigation.TargetEntityType.ClrType).HasQueryFilter(targetFilter);
          }
        }
      }
    }

    // Configure Collection entity
    modelBuilder.Entity<Collection>(builder =>
    {
      builder.HasOne(c => c.Owner)
        .WithMany()
        .HasForeignKey(c => c.OwnerId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    // Configure MovieCollection entity
    modelBuilder.Entity<MovieCollection>(builder =>
    {
      builder.HasOne(mc => mc.Movie)
        .WithMany(m => m.MovieCollections)
        .HasForeignKey(mc => mc.MovieId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(mc => mc.Collection)
        .WithMany(c => c.MovieCollections)
        .HasForeignKey(mc => mc.CollectionId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(mc => new { mc.CollectionId, mc.MovieId })
        .IsUnique();
    });

    // Configure TvShow entity
    modelBuilder.Entity<TvShow>(builder =>
    {
      builder.HasIndex(t => t.TmdbId)
        .IsUnique();

      builder.Property(t => t.Slug)
        .IsRequired(false);
      builder.HasIndex(t => t.Slug)
        .IsUnique()
        .HasFilter(null)
        .IsUnique(false)
        .HasDatabaseName("IX_TvShows_Slug_NonNull");

      builder.HasMany(t => t.Seasons)
        .WithOne(s => s.TvShow)
        .HasForeignKey(s => s.TvShowId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasMany<API.Features.Genres.Models.MediaGenre>()
        .WithOne()
        .HasForeignKey(mg => mg.MediaId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    // Configure Season entity
    modelBuilder.Entity<Season>(builder =>
    {
      builder.HasOne(s => s.TvShow)
        .WithMany(t => t.Seasons)
        .HasForeignKey(s => s.TvShowId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(s => new { s.TvShowId, s.SeasonNumber })
        .IsUnique();

      builder.HasIndex(s => s.TmdbId)
        .IsUnique();
    });

    // Configure Episode entity
    modelBuilder.Entity<Episode>(builder =>
    {
      builder.HasOne(e => e.Season)
        .WithMany(s => s.Episodes)
        .HasForeignKey(e => e.SeasonId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(e => new { e.SeasonId, e.EpisodeNumber })
        .IsUnique();

      builder.HasIndex(e => e.TmdbId)
        .IsUnique();
    });

    // Configure Person entity
    modelBuilder.Entity<Person>(builder =>
    {
      builder.HasIndex(p => p.TmdbId)
        .IsUnique();
    });

    // Configure MovieCast entity
    modelBuilder.Entity<MovieCast>(builder =>
    {
      builder.HasOne(mc => mc.Movie)
        .WithMany(m => m.Cast)
        .HasForeignKey(mc => mc.MovieId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(mc => mc.Person)
        .WithMany(p => p.MovieCastCredits)
        .HasForeignKey(mc => mc.PersonId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(mc => new { mc.MovieId, mc.PersonId })
        .IsUnique();

      builder.Property(mc => mc.CreditId)
        .IsRequired(false);
      builder.HasIndex(mc => mc.CreditId)
        .IsUnique()
        .HasFilter(null)
        .HasDatabaseName("IX_MovieCast_CreditId_NonNull");
    });

    // Configure MovieCrew entity
    modelBuilder.Entity<MovieCrew>(builder =>
    {
      builder.HasOne(mc => mc.Movie)
        .WithMany(m => m.Crew)
        .HasForeignKey(mc => mc.MovieId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(mc => mc.Person)
        .WithMany(p => p.MovieCrewCredits)
        .HasForeignKey(mc => mc.PersonId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.Property(mc => mc.Job)
        .IsRequired(false);
      builder.HasIndex(mc => new { mc.MovieId, mc.PersonId, mc.Job })
        .IsUnique()
        .HasFilter(null)
        .HasDatabaseName("IX_MovieCrew_MovieId_PersonId_Job_NonNull");

      builder.Property(mc => mc.CreditId)
        .IsRequired(false);
      builder.HasIndex(mc => mc.CreditId)
        .IsUnique()
        .HasFilter(null)
        .HasDatabaseName("IX_MovieCrew_CreditId_NonNull");
    });

    // Configure TvShowCast entity
    modelBuilder.Entity<TvShowCast>(builder =>
    {
      builder.HasOne(tc => tc.TvShow)
        .WithMany(t => t.Cast)
        .HasForeignKey(tc => tc.TvShowId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(tc => tc.Person)
        .WithMany(p => p.TvShowCastCredits)
        .HasForeignKey(tc => tc.PersonId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(tc => new { tc.TvShowId, tc.PersonId })
        .IsUnique();

      builder.Property(tc => tc.CreditId)
        .IsRequired(false);
      builder.HasIndex(tc => tc.CreditId)
        .IsUnique()
        .HasFilter(null)
        .HasDatabaseName("IX_TvShowCast_CreditId_NonNull");
    });

    // Configure TvShowCrew entity
    modelBuilder.Entity<TvShowCrew>(builder =>
    {
      builder.HasOne(tc => tc.TvShow)
        .WithMany(t => t.Crew)
        .HasForeignKey(tc => tc.TvShowId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(tc => tc.Person)
        .WithMany(p => p.TvShowCrewCredits)
        .HasForeignKey(tc => tc.PersonId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.Property(tc => tc.Job)
        .IsRequired(false);
      builder.HasIndex(tc => new { tc.TvShowId, tc.PersonId, tc.Job })
        .IsUnique()
        .HasFilter(null)
        .HasDatabaseName("IX_TvShowCrew_TvShowId_PersonId_Job_NonNull");

      builder.Property(tc => tc.CreditId)
        .IsRequired(false);
      builder.HasIndex(tc => tc.CreditId)
        .IsUnique()
        .HasFilter(null)
        .HasDatabaseName("IX_TvShowCrew_CreditId_NonNull");
    });

    // Configure Genre entity
    modelBuilder.Entity<Genre>(builder =>
    {
      builder.HasIndex(g => g.TmdbId)
        .IsUnique();
    });

    // Configure FeatureFlag entity
    modelBuilder.Entity<FeatureFlag>(builder =>
    {
      builder.Property(e => e.Configuration)
          .HasColumnType("jsonb");
    });

    // Configure AbTest entity
    modelBuilder.Entity<AbTest>(builder =>
    {
      builder.Property(e => e.VariantConfigurations)
          .HasColumnType("jsonb");

      builder.Property(e => e.VariantDistribution)
          .HasColumnType("jsonb");
    });

    // Configure MediaGenre entity
    modelBuilder.Entity<API.Features.Genres.Models.MediaGenre>(builder =>
    {
      builder.HasKey(mg => new { mg.MediaId, mg.GenreId });

      builder.HasOne(mg => mg.Genre)
        .WithMany(g => g.MediaGenres)
        .HasForeignKey(mg => mg.GenreId)
        .OnDelete(DeleteBehavior.Cascade);

      // We can't directly reference MediaEntity since it's not mapped to a table
      // Instead, we'll configure the relationships in Movie and TvShow entities
    });

    // Update Movie entity to include MediaGenre relationship
    modelBuilder.Entity<Movie>(builder =>
    {
      builder.HasIndex(m => m.TmdbId)
        .IsUnique();

      builder.Property(m => m.Slug)
        .IsRequired(false);
      builder.HasIndex(m => m.Slug)
        .IsUnique()
        .HasFilter(null)
        .IsUnique(false)
        .HasDatabaseName("IX_Movies_Slug_NonNull");

      builder.HasMany<API.Features.Genres.Models.MediaGenre>()
        .WithOne()
        .HasForeignKey(mg => mg.MediaId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    // Configure Language entity
    modelBuilder.Entity<Language>(builder =>
    {
      builder.HasIndex(l => l.IsoCode)
        .IsUnique();

      builder.Property(l => l.IsoCode)
        .HasMaxLength(10)
        .IsRequired();

      builder.Property(l => l.Name)
        .HasMaxLength(100)
        .IsRequired();

      builder.Property(l => l.FlagIcon)
        .HasMaxLength(50);
    });

    // Apply configurations
    modelBuilder.ApplyConfiguration(new DownloadConfiguration());
    modelBuilder.ApplyConfiguration(new PageConfiguration());
  }

  private static LambdaExpression ConvertFilterExpression(Type entityType, string propertyName, bool value)
  {
    var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
    var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);
    var constant = System.Linq.Expressions.Expression.Constant(value);
    var equation = System.Linq.Expressions.Expression.Equal(property, constant);
    return System.Linq.Expressions.Expression.Lambda(equation, parameter);
  }
}