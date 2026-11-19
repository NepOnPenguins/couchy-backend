using Couchy.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Couchy.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Username of the logged-in user, read from the JWT subject.</summary>
    protected string CurrentUsername =>
        User.Identity?.Name ?? throw ApiException.Unauthorized("Not authenticated");
}
