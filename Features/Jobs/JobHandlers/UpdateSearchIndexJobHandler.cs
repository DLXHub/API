using System.Text.Json;
using API.Features.Jobs.Models;
using API.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace API.Features.Jobs.JobHandlers;

public class UpdateSearchIndexJobHandler : IJobHandler
{
  private readonly ApplicationDbContext _context;
  private readonly IDistributedCache _cache;
  private readonly ILogger<UpdateSearchIndexJobHandler> _logger;

  public UpdateSearchIndexJobHandler(
      ApplicationDbContext context,
      IDistributedCache cache,
      ILogger<UpdateSearchIndexJobHandler> logger)
  {
    _context = context;
    _cache = cache;
    _logger = logger;
  }

  public async Task ExecuteAsync(Job job)
  {
    try
    {
      _logger.LogInformation("Starting search index update...");

      var movies = await _context.Movies
          .Include(m => m.Genres).ThenInclude(mg => mg.Genre)
          .AsNoTracking()
          .ToListAsync();

      var tvShows = await _context.TvShows
          .Include(t => t.Genres).ThenInclude(mg => mg.Genre)
          .AsNoTracking()
          .ToListAsync();

      var pages = await _context.Pages
          .Where(p => p.IsPublished)
          .AsNoTracking()
          .ToListAsync();

      var searchDocuments = new List<SearchDocument>();

      foreach (var movie in movies)
      {
        searchDocuments.Add(new SearchDocument
        {
          Id = movie.Id,
          Type = SearchDocumentType.Movie,
          Title = movie.Title,
          Description = movie.Overview,
          Keywords = movie.Genres.Select(g => g.Genre.Name).ToList(),
          Url = $"/movies/{movie.Slug}",
          LastModified = movie.ModifiedOn ?? movie.CreatedOn
        });
      }

      foreach (var tvShow in tvShows)
      {
        searchDocuments.Add(new SearchDocument
        {
          Id = tvShow.Id,
          Type = SearchDocumentType.TvShow,
          Title = tvShow.Name,
          Description = tvShow.Overview,
          Keywords = tvShow.Genres.Select(g => g.Genre.Name).ToList(),
          Url = $"/tv-shows/{tvShow.Slug}",
          LastModified = tvShow.ModifiedOn ?? tvShow.CreatedOn
        });
      }

      foreach (var page in pages)
      {
        searchDocuments.Add(new SearchDocument
        {
          Id = page.Id,
          Type = SearchDocumentType.Page,
          Title = page.Title,
          Description = page.MetaDescription,
          Keywords = new List<string>(),
          Url = $"/pages/{page.Slug}",
          LastModified = page.ModifiedOn ?? page.CreatedOn
        });
      }

      var cacheKey = "search-index";
      var jsonData = JsonSerializer.Serialize(searchDocuments);
      var options = new DistributedCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
      };
      await _cache.SetStringAsync(cacheKey, jsonData, options);

      _logger.LogInformation("Search index updated successfully with {Count} documents", searchDocuments.Count);

      if (job.Parameters == null)
      {
        job.Parameters = new Dictionary<string, string>();
      }
      job.Parameters["IndexedDocuments"] = searchDocuments.Count.ToString();
      job.Parameters["LastIndexTime"] = DateTime.UtcNow.ToString("O");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating search index");
      throw;
    }
  }
}

public class SearchDocument
{
  public Guid Id { get; set; }
  public SearchDocumentType Type { get; set; }
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public List<string> Keywords { get; set; } = new();
  public string Url { get; set; } = string.Empty;
  public DateTime LastModified { get; set; }
}

public enum SearchDocumentType
{
  Movie,
  TvShow,
  Page
}