using Couchy.Api.Enums;

namespace Couchy.Api.Dtos;

public class StayRequestCreateRequest
{
    public long? HostProfileId { get; set; }
    public string? InitialMessage { get; set; }
}

public class StayRequestResponse
{
    public long Id { get; set; }

    public long TravelerProfileId { get; set; }
    public string? TravelerUsername { get; set; }

    public long HostProfileId { get; set; }
    public string? HostUsername { get; set; }

    public string? InitialMessage { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
