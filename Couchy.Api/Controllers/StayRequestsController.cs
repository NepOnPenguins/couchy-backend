using Couchy.Api.Dtos;
using Couchy.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Couchy.Api.Controllers;

// The route is "messages" because that is the path the Spring backend used.
[Route("api/v1/messages")]
[Authorize]
public class StayRequestsController : ApiControllerBase
{
    private readonly StayRequestService _stayRequestService;

    public StayRequestsController(StayRequestService stayRequestService)
    {
        _stayRequestService = stayRequestService;
    }

    // requests I sent
    [HttpGet("surfing")]
    public async Task<ActionResult<List<StayRequestResponse>>> GetOutgoingRequests()
    {
        return Ok(await _stayRequestService.GetOutgoingRequestsAsync(CurrentUsername));
    }

    // requests I received
    [HttpGet("hosting")]
    public async Task<ActionResult<List<StayRequestResponse>>> GetIncomingRequests()
    {
        return Ok(await _stayRequestService.GetIncomingRequestsAsync(CurrentUsername));
    }

    [HttpPost]
    public async Task<ActionResult<StayRequestResponse>> CreateStayRequest([FromBody] StayRequestCreateRequest request)
    {
        return Ok(await _stayRequestService.CreateStayRequestAsync(CurrentUsername, request));
    }

    [HttpPost("{requestId:long}/accept")]
    public async Task<ActionResult<StayRequestResponse>> Accept(long requestId)
    {
        return Ok(await _stayRequestService.AcceptRequestAsync(CurrentUsername, requestId));
    }

    [HttpPost("{requestId:long}/reject")]
    public async Task<ActionResult<StayRequestResponse>> Reject(long requestId)
    {
        return Ok(await _stayRequestService.RejectRequestAsync(CurrentUsername, requestId));
    }
}
