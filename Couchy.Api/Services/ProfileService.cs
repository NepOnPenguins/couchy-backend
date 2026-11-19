using Couchy.Api.Common;
using Couchy.Api.Data;
using Couchy.Api.Dtos;
using Couchy.Api.Entities;
using Couchy.Api.Enums;
using Couchy.Api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Couchy.Api.Services;

public class ProfileService
{
    private readonly CouchyDbContext _db;

    public ProfileService(CouchyDbContext db)
    {
        _db = db;
    }

    public async Task<ProfileResponse> CreateProfileAsync(string username, CreateProfileRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username)
                   ?? throw ApiException.NotFound("User not found");

        if (await _db.Profiles.AnyAsync(p => p.UserId == user.UserId))
            throw ApiException.Conflict("Profile already exists for that user");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw ApiException.BadRequest("First name is required");
        if (string.IsNullOrWhiteSpace(request.LastName))
            throw ApiException.BadRequest("Last name is required");
        if (string.IsNullOrWhiteSpace(request.About))
            throw ApiException.BadRequest("About section is required");

        // Only the required fields are set here; the rest comes through the edit endpoints.
        var profile = new Profile
        {
            User = user,
            FirstName = request.FirstName,
            LastName = request.LastName,
            About = request.About
        };

        // optional
        if (request.Gender != null) profile.Gender = request.Gender;
        if (request.Pronouns != null) profile.Pronouns = request.Pronouns;
        if (request.Languages != null) profile.Languages = request.Languages;
        if (request.HostingStatus != null) profile.HostingStatus = request.HostingStatus.Value;
        if (request.MeetingStatus != null) profile.MeetingStatus = request.MeetingStatus.Value;

        _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        return profile.ToResponse();
    }

    public async Task<ProfileResponse> GetMyProfileAsync(string username)
    {
        var profile = await GetProfileOrThrowAsync(username);
        return profile.ToResponse();
    }

    public async Task<ProfileResponse> UpdateAboutMyProfileAsync(string username, UpdateAboutProfileRequest request)
    {
        var profile = await GetProfileOrThrowAsync(username);

        if (request.FirstName != null)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw ApiException.BadRequest("First name cannot be blank");
            profile.FirstName = request.FirstName;
        }

        if (request.LastName != null)
        {
            if (string.IsNullOrWhiteSpace(request.LastName))
                throw ApiException.BadRequest("Last name cannot be blank");
            profile.LastName = request.LastName;
        }

        if (request.About != null)
        {
            if (string.IsNullOrWhiteSpace(request.About))
                throw ApiException.BadRequest("About cannot be blank");

            var wordCount = request.About.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
            if (wordCount < 10)
                throw ApiException.BadRequest("About must contain at least 10 words");

            profile.About = request.About;
        }

        if (request.Gender != null) profile.Gender = request.Gender;
        if (request.Pronouns != null) profile.Pronouns = request.Pronouns;
        if (request.Languages != null) profile.Languages = request.Languages;

        await _db.SaveChangesAsync();
        return profile.ToResponse();
    }

    public async Task<ProfileResponse> UpdateHomeMyProfileAsync(string username, UpdateHomeProfileRequest request)
    {
        var profile = await GetProfileOrThrowAsync(username);

        if (request.Location != null)
        {
            var loc = request.Location;

            if (string.IsNullOrWhiteSpace(loc.City))
                throw ApiException.BadRequest("City cannot be blank");
            if (string.IsNullOrWhiteSpace(loc.Country))
                throw ApiException.BadRequest("Country cannot be blank");

            // The whole location is replaced by what was sent.
            profile.Location ??= new Location();
            profile.Location.City = loc.City;
            profile.Location.Country = loc.Country;
            profile.Location.Latitude = loc.Latitude;
            profile.Location.Longitude = loc.Longitude;
            profile.Location.District = null;
        }

        if (request.HomeDetails != null)
        {
            // Home details are merged: only the fields that were sent change.
            profile.HomeDetails ??= new HomeDetails();
            var home = profile.HomeDetails;
            var homeReq = request.HomeDetails;

            if (homeReq.MaxGuests != null) home.MaxGuests = homeReq.MaxGuests;
            if (homeReq.HasWifi != null) home.HasWifi = homeReq.HasWifi;
            if (homeReq.HasKitchen != null) home.HasKitchen = homeReq.HasKitchen;
            if (homeReq.HasWashingMachine != null) home.HasWashingMachine = homeReq.HasWashingMachine;
            if (homeReq.HasHeating != null) home.HasHeating = homeReq.HasHeating;
            if (homeReq.HasAirConditioning != null) home.HasAirConditioning = homeReq.HasAirConditioning;
            if (homeReq.HasBathroom != null) home.HasBathroom = homeReq.HasBathroom;
            if (homeReq.PetsAllowed != null) home.PetsAllowed = homeReq.PetsAllowed;
            if (homeReq.SmokingAllowed != null) home.SmokingAllowed = homeReq.SmokingAllowed;

            if (!string.IsNullOrWhiteSpace(homeReq.SleepingArrangement))
                home.SleepingArrangement = homeReq.SleepingArrangement;
            if (!string.IsNullOrWhiteSpace(homeReq.HouseRules))
                home.HouseRules = homeReq.HouseRules;
        }

        if (request.HostingStatus != null) profile.HostingStatus = request.HostingStatus.Value;
        if (request.MeetingStatus != null) profile.MeetingStatus = request.MeetingStatus.Value;

        await _db.SaveChangesAsync();
        return profile.ToResponse();
    }

    /// <summary>
    /// Lists profiles, optionally filtered by city and hosting status.
    /// An unknown hosting status is ignored instead of causing an error.
    /// </summary>
    public async Task<List<ProfileResponse>> GetAllProfilesAsync(string? city, string? hostingStatus)
    {
        IQueryable<Profile> query = _db.Profiles.AsNoTracking().Include(p => p.User);

        if (city != null)
            query = query.Where(p => p.Location!.City == city);

        var status = ParseHostingStatus(hostingStatus);
        if (status != null)
        {
            var wanted = status.Value;
            query = query.Where(p => p.HostingStatus == wanted);
        }

        var profiles = await query.OrderBy(p => p.ProfileId).ToListAsync();
        return profiles.Select(p => p.ToResponse()).ToList();
    }

    public async Task<ProfileResponse> GetProfileByIdAsync(long profileId)
    {
        var profile = await _db.Profiles.AsNoTracking()
                          .Include(p => p.User)
                          .FirstOrDefaultAsync(p => p.ProfileId == profileId)
                      ?? throw ApiException.NotFound("Profile not found");

        return profile.ToResponse();
    }

    /// <summary>Loads the profile of the given user (tracked, so it can be edited).</summary>
    public async Task<Profile> GetProfileOrThrowAsync(string username)
    {
        return await _db.Profiles
                   .Include(p => p.User)
                   .FirstOrDefaultAsync(p => p.User.Username == username)
               ?? throw ApiException.NotFound("Profile must be created first");
    }

    private static HostingStatus? ParseHostingStatus(string? value)
    {
        // Exact name match only, like Java's HostingStatus.valueOf.
        if (value != null && Enum.GetNames<HostingStatus>().Contains(value))
            return Enum.Parse<HostingStatus>(value);

        return null;
    }
}
