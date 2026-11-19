using Couchy.Api.Auth;
using Couchy.Api.Common;
using Couchy.Api.Data;
using Couchy.Api.Dtos;
using Couchy.Api.Entities;
using Couchy.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace Couchy.Api.Services;

public class AuthService
{
    private readonly CouchyDbContext _db;
    private readonly JwtService _jwtService;

    public AuthService(CouchyDbContext db, JwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrEmpty(request.Password))
        {
            throw ApiException.BadRequest("Username, email and password are required");
        }

        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            throw ApiException.Conflict("username is taken");

        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            throw ApiException.Conflict("email is taken");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = PasswordHasher.Hash(request.Password),
            Role = Role.ROLE_USER
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthenticationResponse { Token = _jwtService.GenerateToken(user) };
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
    {
        User? user = null;
        if (!string.IsNullOrEmpty(request.Username))
            user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null
            || string.IsNullOrEmpty(request.Password)
            || !PasswordHasher.Verify(request.Password, user.Password))
        {
            throw ApiException.Unauthorized("Invalid username or password");
        }

        return new AuthenticationResponse { Token = _jwtService.GenerateToken(user) };
    }
}
