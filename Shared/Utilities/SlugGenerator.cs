using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Shared.Utilities;

/// <summary>
/// Utility class for generating URL-friendly slugs.
/// </summary>
public static class SlugGenerator
{
  /// <summary>
  /// Generates a URL-friendly slug from the given text.
  /// </summary>
  /// <param name="text">The text to convert to a slug.</param>
  /// <param name="maxLength">The maximum length of the slug (default: 100).</param>
  /// <returns>A URL-friendly slug.</returns>
  public static string GenerateSlug(string text, int maxLength = 100)
  {
    if (string.IsNullOrWhiteSpace(text))
      return string.Empty;

    // Remove diacritics (accents)
    var normalizedString = text.Normalize(NormalizationForm.FormD);
    var stringBuilder = new StringBuilder();

    foreach (var c in normalizedString)
    {
      var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
      if (unicodeCategory != UnicodeCategory.NonSpacingMark)
      {
        stringBuilder.Append(c);
      }
    }

    // Convert to lowercase and trim
    var slug = stringBuilder.ToString().ToLowerInvariant().Trim();

    // Replace spaces with hyphens and remove invalid characters
    slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
    slug = Regex.Replace(slug, @"\s+", "-");
    slug = Regex.Replace(slug, @"-+", "-");

    // Trim to max length
    if (slug.Length > maxLength)
      slug = slug.Substring(0, maxLength).TrimEnd('-');

    return slug;
  }

  /// <summary>
  /// Generates a unique slug by appending a number if necessary.
  /// </summary>
  /// <param name="text">The text to convert to a slug.</param>
  /// <param name="existingSlugs">A collection of existing slugs to check against.</param>
  /// <param name="maxLength">The maximum length of the slug (default: 100).</param>
  /// <returns>A unique URL-friendly slug.</returns>
  public static string GenerateUniqueSlug(string text, IEnumerable<string> existingSlugs, int maxLength = 100)
  {
    var baseSlug = GenerateSlug(text, maxLength);

    if (string.IsNullOrEmpty(baseSlug))
      return string.Empty;

    var slug = baseSlug;
    var existingSlugsList = existingSlugs.ToList();
    var counter = 1;

    // If the slug already exists, append a number
    while (existingSlugsList.Contains(slug, StringComparer.OrdinalIgnoreCase))
    {
      var suffix = $"-{counter}";
      var maxBaseLength = maxLength - suffix.Length;

      if (baseSlug.Length > maxBaseLength)
        baseSlug = baseSlug.Substring(0, maxBaseLength).TrimEnd('-');

      slug = $"{baseSlug}{suffix}";
      counter++;
    }

    return slug;
  }
}