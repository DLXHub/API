using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Features.UserManagement.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Features.UserManagement.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<ApiResponse<LoginResponse>>;

public record LoginResponse(string Token, UserDto User);

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ApiResponse<LoginResponse>>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly JwtSettings _jwtSettings;

  public LoginUserCommandHandler(
      UserManager<ApplicationUser> userManager,
      IOptions<JwtSettings> jwtSettings)
  {
    _userManager = userManager;
    _jwtSettings = jwtSettings.Value;
  }

  public async Task<ApiResponse<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user == null)
    {
      return ApiResponse<LoginResponse>.CreateError("Invalid credentials");
    }

    var result = await _userManager.CheckPasswordAsync(user, request.Password);
    if (!result)
    {
      return ApiResponse<LoginResponse>.CreateError("Invalid credentials");
    }

    // Update last login
    user.LastLoginAt = DateTime.UtcNow;
    await _userManager.UpdateAsync(user);

    // Generate JWT token
    var token = await GenerateJwtToken(user);

    var response = new LoginResponse(
        Token: token,
        User: new UserDto
        {
          Id = user.Id,
          Email = user.Email!,
          UserName = user.UserName!,
          FirstName = user.FirstName,
          LastName = user.LastName,
          CreatedAt = user.CreatedAt,
          LastLoginAt = user.LastLoginAt
        });

    return ApiResponse<LoginResponse>.CreateSuccess(response);
  }

  private async Task<string> GenerateJwtToken(ApplicationUser user)
  {
    var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName!)
        };

    var userRoles = await _userManager.GetRolesAsync(user);
    claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}