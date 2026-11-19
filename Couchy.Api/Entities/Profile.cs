using Couchy.Api.Enums;

namespace Couchy.Api.Entities;

public class Profile
{
    public long ProfileId { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? Pronouns { get; set; }
    public string? Languages { get; set; }

    // MAY_HOST and MAY_MEET by default
    public HostingStatus HostingStatus { get; set; } = HostingStatus.MAY_HOST;
    public MeetingStatus MeetingStatus { get; set; } = MeetingStatus.MAY_MEET;

    public string? About { get; set; }

    // Stored as columns of the profiles table (JPA @Embedded -> EF Core owned type).
    public Location? Location { get; set; }
    public HomeDetails? HomeDetails { get; set; }
}
