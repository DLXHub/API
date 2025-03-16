using API.Features.Genres.Models;

namespace API.Features.Genres.Models;

public class MediaGenre
{
  public Guid MediaId { get; set; }
  public Guid GenreId { get; set; }
  public Genre Genre { get; set; } = null!;
}