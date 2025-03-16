using API.Features.UserManagement.Models;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace API.Features.UserManagement.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string UserName,
    string Password,
    string? FirstName,
    string? LastName) : IRequest<ApiResponse<UserDto>>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<UserDto>>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task<ApiResponse<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
  {
    var user = new ApplicationUser
    {
      UserName = request.UserName,
      Email = request.Email,
      FirstName = request.FirstName,
      LastName = request.LastName,
      CreatedAt = DateTime.UtcNow
    };

    var result = await _userManager.CreateAsync(user, request.Password);

    if (!result.Succeeded)
    {
      return ApiResponse<UserDto>.CreateError(
          "Registration failed",
          result.Errors.Select(e => e.Description).ToList());
    }

    return ApiResponse<UserDto>.CreateSuccess(new UserDto
    {
      Id = user.Id,
      Email = user.Email!,
      UserName = user.UserName!,
      FirstName = user.FirstName,
      LastName = user.LastName,
      CreatedAt = user.CreatedAt,
      LastLoginAt = user.LastLoginAt
    });
  }
}