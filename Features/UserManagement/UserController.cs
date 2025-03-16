using API.Features.UserManagement.Commands.DeleteUser;
using API.Features.UserManagement.Commands.LoginUser;
using API.Features.UserManagement.Commands.RegisterUser;
using API.Features.UserManagement.Commands.UpdateUser;
using API.Features.UserManagement.Models;
using API.Features.UserManagement.Queries.GetUser;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.UserManagement;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
  private readonly IMediator _mediator;

  public UserController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Register a new user
  /// </summary>
  [HttpPost("register")]
  [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<UserDto>>> Register(RegisterRequest request)
  {
    var command = new RegisterUserCommand(
        request.Email,
        request.UserName,
        request.Password,
        request.FirstName,
        request.LastName);

    var result = await _mediator.Send(command);
    return result.Success ? Ok(result) : BadRequest(result);
  }

  /// <summary>
  /// Login user
  /// </summary>
  [HttpPost("login")]
  [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request)
  {
    var command = new LoginUserCommand(request.Email, request.Password);
    var result = await _mediator.Send(command);
    return result.Success ? Ok(result) : BadRequest(result);
  }

  /// <summary>
  /// Get current user profile
  /// </summary>
  [Authorize]
  [HttpGet("profile")]
  [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
    {
      return Unauthorized();
    }

    var query = new GetUserQuery(userId);
    var result = await _mediator.Send(query);
    return result.Success ? Ok(result) : NotFound(result);
  }

  /// <summary>
  /// Update user profile
  /// </summary>
  [Authorize]
  [HttpPut("profile")]
  [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile(UpdateUserRequest request)
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
    {
      return Unauthorized();
    }

    var command = new UpdateUserCommand(
        userId,
        request.FirstName,
        request.LastName,
        request.CurrentPassword,
        request.NewPassword);

    var result = await _mediator.Send(command);
    return result.Success ? Ok(result) : BadRequest(result);
  }

  /// <summary>
  /// Delete user account
  /// </summary>
  [Authorize]
  [HttpDelete]
  [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<bool>>> DeleteAccount()
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
    {
      return Unauthorized();
    }

    var command = new DeleteUserCommand(userId);
    var result = await _mediator.Send(command);
    return result.Success ? Ok(result) : BadRequest(result);
  }
}