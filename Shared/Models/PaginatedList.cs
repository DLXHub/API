using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Shared.Models;

/// <summary>
/// A generic class for paginated lists
/// </summary>
/// <typeparam name="T">The type of items in the list</typeparam>
public class PaginatedList<T>
{
  /// <summary>
  /// The items in the current page
  /// </summary>
  public List<T> Items { get; }

  /// <summary>
  /// The current page number (1-based)
  /// </summary>
  public int PageNumber { get; }

  /// <summary>
  /// The total number of pages
  /// </summary>
  public int TotalPages { get; }

  /// <summary>
  /// The total number of items across all pages
  /// </summary>
  public int TotalCount { get; }

  /// <summary>
  /// The number of items per page
  /// </summary>
  public int PageSize { get; }

  /// <summary>
  /// Whether there is a previous page
  /// </summary>
  public bool HasPreviousPage => PageNumber > 1;

  /// <summary>
  /// Whether there is a next page
  /// </summary>
  public bool HasNextPage => PageNumber < TotalPages;

  /// <summary>
  /// Creates a new paginated list
  /// </summary>
  /// <param name="items">The items in the current page</param>
  /// <param name="count">The total number of items across all pages</param>
  /// <param name="pageNumber">The current page number (1-based)</param>
  /// <param name="pageSize">The number of items per page</param>
  public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
  {
    PageNumber = pageNumber;
    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    TotalCount = count;
    PageSize = pageSize;
    Items = items;
  }

  /// <summary>
  /// Creates a new paginated list from a queryable source
  /// </summary>
  /// <param name="source">The queryable source</param>
  /// <param name="pageNumber">The page number (1-based)</param>
  /// <param name="pageSize">The page size</param>
  /// <returns>A paginated list</returns>
  public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
  {
    var count = await source.CountAsync();

    // Ensure pageNumber is at least 1
    pageNumber = Math.Max(1, pageNumber);

    // Ensure pageSize is at least 1
    pageSize = Math.Max(1, pageSize);

    var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    return new PaginatedList<T>(items, count, pageNumber, pageSize);
  }
}