namespace API.Features.UserManagement.Models;

public record RegisterRequest(
    string Email,
    string UserName,
    string Password,
    string? FirstName,
    string? LastName);