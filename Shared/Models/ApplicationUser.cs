using Microsoft.AspNetCore.Identity;

namespace API.Shared.Models;

public class ApplicationUser : IdentityUser
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? LastLoginAt { get; set; }
}