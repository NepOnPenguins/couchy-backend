using Couchy.Api.Auth;
using Couchy.Api.Common;
using Couchy.Api.Data;
using Couchy.Api.Dtos;
using Couchy.Api.Entities;
using Couchy.Api.Enums;
using Couchy.Api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Couchy.Api.Services;

public class UserService
{
    private readonly CouchyDbContext _db;

    public UserService(CouchyDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserResponse>> GetUsersAsync()
    {
        var users = await _db.Users.AsNoTracking().OrderBy(u => u.UserId).ToListAsync();
        return users.Select(u => u.ToResponse()).ToList();
    }

    public async Task<UserResponse> AddUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrEmpty(request.Password))
        {
            throw ApiException.BadRequest("Username, email and password are required");
        }

        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            throw ApiException.Conflict("email is taken");

        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            throw ApiException.Conflict("username is taken");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = PasswordHasher.Hash(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user.ToResponse();
    }

    /// <summary>
    /// Changes only the values that were sent, are longer than 4 characters and differ
    /// from the current value. Username and email must stay unique.
    /// </summary>
    public async Task UpdateUserAsync(long userId, string? username, string? email, string? password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId)
                   ?? throw ApiException.NotFound($"user with id: {userId} does not exist");

        if (username != null && username.Length > 4 && user.Username != username)
        {
            var other = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (other != null && other.UserId != userId)
                throw ApiException.Conflict($"username is already taken by the user with id: {other.UserId}");

            user.Username = username;
        }

        if (email != null && email.Length > 4 && user.Email != email)
        {
            var other = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (other != null && other.UserId != userId)
                throw ApiException.Conflict($"email is already taken by the user with id: {other.UserId}");

            user.Email = email;
        }

        if (password != null && password.Length > 4)
            user.Password = PasswordHasher.Hash(password);

        await _db.SaveChangesAsync();
    }

    /// <summary>Soft delete: the user is marked DELETED and can no longer host or meet.</summary>
    public async Task DeleteUserAsync(long userId)
    {
        var user = await _db.Users
                       .Include(u => u.Profile)
                       .FirstOrDefaultAsync(u => u.UserId == userId)
                   ?? throw ApiException.NotFound($"User not found with id: {userId}");

        if (user.Status == UserStatus.DELETED)
            throw ApiException.Conflict("User already deleted");

        user.Status = UserStatus.DELETED;

        if (user.Profile != null)
        {
            user.Profile.HostingStatus = HostingStatus.CANT_HOST;
            user.Profile.MeetingStatus = MeetingStatus.CANT_MEET;
        }

        await _db.SaveChangesAsync();
    }
}
