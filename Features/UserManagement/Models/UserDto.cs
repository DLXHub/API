namespace API.Features.UserManagement.Models;

public class UserDto
{
  public string Id { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string UserName { get; set; } = default!;
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? LastLoginAt { get; set; }
}