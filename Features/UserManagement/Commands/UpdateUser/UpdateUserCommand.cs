using API.Features.UserManagement.Models;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace API.Features.UserManagement.Commands.UpdateUser;

public record UpdateUserCommand(
    string UserId,
    string? FirstName,
    string? LastName,
    string? CurrentPassword,
    string? NewPassword) : IRequest<ApiResponse<UserDto>>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _userManager.FindByIdAsync(request.UserId);
    if (user == null)
    {
      return ApiResponse<UserDto>.CreateError("User not found");
    }

    // Update basic info
    if (request.FirstName != null) user.FirstName = request.FirstName;
    if (request.LastName != null) user.LastName = request.LastName;

    // Update password if provided
    if (request.CurrentPassword != null && request.NewPassword != null)
    {
      var passwordCheck = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
      if (!passwordCheck)
      {
        return ApiResponse<UserDto>.CreateError("Current password is incorrect");
      }

      var passwordResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
      if (!passwordResult.Succeeded)
      {
        return ApiResponse<UserDto>.CreateError(
            "Password update failed",
            passwordResult.Errors.Select(e => e.Description).ToList());
      }
    }

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
    {
      return ApiResponse<UserDto>.CreateError(
          "Update failed",
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