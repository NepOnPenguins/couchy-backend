using Couchy.Api.Common;
using Couchy.Api.Enums;

namespace Couchy.Api.Entities;

public class StayRequest
{
    public long Id { get; set; }

    // The profile that sent the request
    public long TravelerProfileId { get; set; }
    public Profile TravelerProfile { get; set; } = null!;

    // The profile that received the request
    public long HostProfileId { get; set; }
    public Profile HostProfile { get; set; } = null!;

    public RequestStatus Status { get; set; } = RequestStatus.PENDING;

    public DateTime CreatedAt { get; set; } = Clock.Now();

    public string InitialMessage { get; set; } = string.Empty;
}
