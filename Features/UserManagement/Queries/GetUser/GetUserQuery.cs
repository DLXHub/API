using API.Features.UserManagement.Models;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace API.Features.UserManagement.Queries.GetUser;

public record GetUserQuery(string UserId) : IRequest<ApiResponse<UserDto>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApiResponse<UserDto>>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public GetUserQueryHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task<ApiResponse<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
  {
    var user = await _userManager.FindByIdAsync(request.UserId);
    if (user == null)
    {
      return ApiResponse<UserDto>.CreateError("User not found");
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