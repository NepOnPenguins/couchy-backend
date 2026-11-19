using Couchy.Api.Dtos;
using Couchy.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Couchy.Api.Controllers;

// NOTE: as in the Spring backend, any logged-in user can call these endpoints,
// including updating or deleting another user. See "Open decisions" in the README.
[Route("api/v1/users")]
[Authorize]
public class UsersController : ApiControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        return Ok(await _userService.GetUsersAsync());
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> AddUser([FromBody] CreateUserRequest request)
    {
        var created = await _userService.AddUserAsync(request);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{userId:long}")]
    public async Task<IActionResult> UpdateUser(
        long userId,
        [FromQuery] string? username,
        [FromQuery] string? email,
        [FromQuery] string? password)
    {
        await _userService.UpdateUserAsync(userId, username, email, password);
        return NoContent();
    }

    [HttpDelete("{userId:long}")]
    public async Task<IActionResult> DeleteUser(long userId)
    {
        await _userService.DeleteUserAsync(userId);
        return NoContent();
    }
}
