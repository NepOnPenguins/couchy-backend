using Couchy.Api.Dtos;
using Couchy.Api.Entities;

namespace Couchy.Api.Mappers;

public static class StayRequestMapper
{
    public static StayRequestResponse ToResponse(this StayRequest stayRequest)
    {
        return new StayRequestResponse
        {
            Id = stayRequest.Id,
            TravelerProfileId = stayRequest.TravelerProfileId,
            TravelerUsername = stayRequest.TravelerProfile?.User?.Username,
            HostProfileId = stayRequest.HostProfileId,
            HostUsername = stayRequest.HostProfile?.User?.Username,
            InitialMessage = stayRequest.InitialMessage,
            Status = stayRequest.Status,
            CreatedAt = stayRequest.CreatedAt
        };
    }
}
