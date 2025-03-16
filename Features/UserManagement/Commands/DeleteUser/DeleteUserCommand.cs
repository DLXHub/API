using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace API.Features.UserManagement.Commands.DeleteUser;

public record DeleteUserCommand(string UserId) : IRequest<ApiResponse<bool>>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _userManager.FindByIdAsync(request.UserId);
    if (user == null)
    {
      return ApiResponse<bool>.CreateError("User not found");
    }

    var result = await _userManager.DeleteAsync(user);
    if (!result.Succeeded)
    {
      return ApiResponse<bool>.CreateError(
          "Delete failed",
          result.Errors.Select(e => e.Description).ToList());
    }

    return ApiResponse<bool>.CreateSuccess(true, "User deleted successfully");
  }
}