using Couchy.Api.Common;
using Couchy.Api.Data;
using Couchy.Api.Dtos;
using Couchy.Api.Entities;
using Couchy.Api.Enums;
using Couchy.Api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Couchy.Api.Services;

public class StayRequestService
{
    private readonly CouchyDbContext _db;
    private readonly ProfileService _profileService;

    public StayRequestService(CouchyDbContext db, ProfileService profileService)
    {
        _db = db;
        _profileService = profileService;
    }

    /// <summary>Requests other people sent to me (I am the host).</summary>
    public async Task<List<StayRequestResponse>> GetIncomingRequestsAsync(string username)
    {
        var host = await _profileService.GetProfileOrThrowAsync(username);

        var requests = await WithProfiles()
            .AsNoTracking()
            .Where(s => s.HostProfileId == host.ProfileId)
            .OrderBy(s => s.Id)
            .ToListAsync();

        return requests.Select(s => s.ToResponse()).ToList();
    }

    /// <summary>Requests I sent to other people (I am the traveler).</summary>
    public async Task<List<StayRequestResponse>> GetOutgoingRequestsAsync(string username)
    {
        var traveler = await _profileService.GetProfileOrThrowAsync(username);

        var requests = await WithProfiles()
            .AsNoTracking()
            .Where(s => s.TravelerProfileId == traveler.ProfileId)
            .OrderBy(s => s.Id)
            .ToListAsync();

        return requests.Select(s => s.ToResponse()).ToList();
    }

    public async Task<StayRequestResponse> CreateStayRequestAsync(string username, StayRequestCreateRequest request)
    {
        if (request.HostProfileId == null)
            throw ApiException.BadRequest("Host profile ID cannot be null");
        if (string.IsNullOrWhiteSpace(request.InitialMessage))
            throw ApiException.BadRequest("Initial message cannot be blank");

        var traveler = await _profileService.GetProfileOrThrowAsync(username);

        var host = await _db.Profiles
                       .Include(p => p.User)
                       .FirstOrDefaultAsync(p => p.ProfileId == request.HostProfileId.Value)
                   ?? throw ApiException.NotFound("Host not found");

        if (traveler.ProfileId == host.ProfileId)
            throw ApiException.BadRequest("You cannot send request to yourself");

        var stayRequest = new StayRequest
        {
            TravelerProfile = traveler,
            HostProfile = host,
            InitialMessage = request.InitialMessage
        };

        _db.StayRequests.Add(stayRequest);
        await _db.SaveChangesAsync();

        return stayRequest.ToResponse();
    }

    public Task<StayRequestResponse> AcceptRequestAsync(string username, long requestId)
    {
        return DecideAsync(username, requestId, RequestStatus.ACCEPTED, "accept");
    }

    public Task<StayRequestResponse> RejectRequestAsync(string username, long requestId)
    {
        return DecideAsync(username, requestId, RequestStatus.REJECTED, "reject");
    }

    private async Task<StayRequestResponse> DecideAsync(string username, long requestId, RequestStatus newStatus, string action)
    {
        var host = await _profileService.GetProfileOrThrowAsync(username);

        var stayRequest = await WithProfiles().FirstOrDefaultAsync(s => s.Id == requestId)
                          ?? throw ApiException.NotFound("Stay request not found");

        if (stayRequest.HostProfileId != host.ProfileId)
            throw ApiException.Forbidden($"Only host can {action} the request");

        if (stayRequest.Status != RequestStatus.PENDING)
            throw ApiException.Conflict("Request is already processed");

        stayRequest.Status = newStatus;
        await _db.SaveChangesAsync();

        return stayRequest.ToResponse();
    }

    private IQueryable<StayRequest> WithProfiles()
    {
        return _db.StayRequests
            .Include(s => s.TravelerProfile).ThenInclude(p => p.User)
            .Include(s => s.HostProfile).ThenInclude(p => p.User);
    }
}
