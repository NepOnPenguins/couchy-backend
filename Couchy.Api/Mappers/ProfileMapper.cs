using Couchy.Api.Dtos;
using Couchy.Api.Entities;

namespace Couchy.Api.Mappers;

public static class ProfileMapper
{
    public static ProfileResponse ToResponse(this Profile profile)
    {
        return new ProfileResponse
        {
            Username = profile.User?.Username,
            ProfileId = profile.ProfileId,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            About = profile.About,
            Gender = profile.Gender,
            Pronouns = profile.Pronouns,
            Languages = profile.Languages,
            HostingStatus = profile.HostingStatus,
            MeetingStatus = profile.MeetingStatus,
            Location = profile.Location == null
                ? null
                : new LocationResponse
                {
                    City = profile.Location.City,
                    Country = profile.Location.Country,
                    Latitude = profile.Location.Latitude,
                    Longitude = profile.Location.Longitude
                },
            HomeDetails = profile.HomeDetails == null
                ? null
                : new HomeDetailsResponse
                {
                    MaxGuests = profile.HomeDetails.MaxGuests,
                    HasWifi = profile.HomeDetails.HasWifi,
                    HasKitchen = profile.HomeDetails.HasKitchen,
                    HasWashingMachine = profile.HomeDetails.HasWashingMachine,
                    HasHeating = profile.HomeDetails.HasHeating,
                    HasAirConditioning = profile.HomeDetails.HasAirConditioning,
                    HasBathroom = profile.HomeDetails.HasBathroom,
                    PetsAllowed = profile.HomeDetails.PetsAllowed,
                    SmokingAllowed = profile.HomeDetails.SmokingAllowed,
                    SleepingArrangement = profile.HomeDetails.SleepingArrangement,
                    HouseRules = profile.HomeDetails.HouseRules
                }
        };
    }
}
