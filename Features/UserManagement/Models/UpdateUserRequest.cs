namespace API.Features.UserManagement.Models;

public record UpdateUserRequest(
    string? FirstName,
    string? LastName,
    string? CurrentPassword,
    string? NewPassword);