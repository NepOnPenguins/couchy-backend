using Couchy.Api.Enums;

namespace Couchy.Api.Dtos;

// All request properties are nullable on purpose: the services decide what is
// required, and the PATCH endpoints only change the fields that were sent.

public class CreateProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? About { get; set; }

    public string? Gender { get; set; }
    public string? Pronouns { get; set; }
    public string? Languages { get; set; }
    public HostingStatus? HostingStatus { get; set; }
    public MeetingStatus? MeetingStatus { get; set; }
}

public class UpdateAboutProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? About { get; set; }

    public string? Gender { get; set; }
    public string? Pronouns { get; set; }
    public string? Languages { get; set; }
}

public class UpdateHomeProfileRequest
{
    public HomeDetailsRequest? HomeDetails { get; set; }
    public LocationRequest? Location { get; set; }
    public HostingStatus? HostingStatus { get; set; }
    public MeetingStatus? MeetingStatus { get; set; }
}

public class LocationRequest
{
    public string? City { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class HomeDetailsRequest
{
    public int? MaxGuests { get; set; }
    public bool? HasWifi { get; set; }
    public bool? HasKitchen { get; set; }
    public bool? HasWashingMachine { get; set; }
    public bool? HasHeating { get; set; }
    public bool? HasAirConditioning { get; set; }
    public bool? HasBathroom { get; set; }
    public bool? PetsAllowed { get; set; }
    public bool? SmokingAllowed { get; set; }
    public string? SleepingArrangement { get; set; }
    public string? HouseRules { get; set; }
}

public class ProfileResponse
{
    public string? Username { get; set; }
    public long ProfileId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? About { get; set; }

    public LocationResponse? Location { get; set; }
    public HomeDetailsResponse? HomeDetails { get; set; }

    public string? Gender { get; set; }
    public string? Pronouns { get; set; }
    public string? Languages { get; set; }

    public HostingStatus HostingStatus { get; set; }
    public MeetingStatus MeetingStatus { get; set; }
}

public class LocationResponse
{
    public string? City { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class HomeDetailsResponse
{
    public int? MaxGuests { get; set; }
    public bool? HasWifi { get; set; }
    public bool? HasKitchen { get; set; }
    public bool? HasWashingMachine { get; set; }
    public bool? HasHeating { get; set; }
    public bool? HasAirConditioning { get; set; }
    public bool? HasBathroom { get; set; }
    public bool? PetsAllowed { get; set; }
    public bool? SmokingAllowed { get; set; }
    public string? SleepingArrangement { get; set; }
    public string? HouseRules { get; set; }
}
