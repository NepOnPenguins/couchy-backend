using Couchy.Api.Dtos;
using Couchy.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Couchy.Api.Controllers;

[Route("api/v1/profiles")]
[Authorize]
public class ProfilesController : ApiControllerBase
{
    private readonly ProfileService _profileService;

    public ProfilesController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    // create (AUTH)
    [HttpPost("me/create")]
    public async Task<ActionResult<ProfileResponse>> CreateMyProfile([FromBody] CreateProfileRequest request)
    {
        return Ok(await _profileService.CreateProfileAsync(CurrentUsername, request));
    }

    // get my profile (AUTH)
    [HttpGet("me")]
    public async Task<ActionResult<ProfileResponse>> GetMyProfile()
    {
        return Ok(await _profileService.GetMyProfileAsync(CurrentUsername));
    }

    // list / search profiles (public)
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProfileResponse>>> GetAllProfiles(
        [FromQuery] string? city,
        [FromQuery] string? hostingStatus)
    {
        return Ok(await _profileService.GetAllProfilesAsync(city, hostingStatus));
    }

    // (AUTH)
    [HttpPatch("edit/about")]
    public async Task<IActionResult> UpdateAboutMyProfile([FromBody] UpdateAboutProfileRequest request)
    {
        await _profileService.UpdateAboutMyProfileAsync(CurrentUsername, request);
        return Ok(new { message = "profile updated successfully" });
    }

    // (AUTH)
    [HttpPatch("edit/home")]
    public async Task<IActionResult> UpdateHomeMyProfile([FromBody] UpdateHomeProfileRequest request)
    {
        await _profileService.UpdateHomeMyProfileAsync(CurrentUsername, request);
        return Ok(new { message = "profile updated successfully" });
    }

    // public, numeric ids only
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProfileResponse>> GetProfileById(long id)
    {
        return Ok(await _profileService.GetProfileByIdAsync(id));
    }
}
