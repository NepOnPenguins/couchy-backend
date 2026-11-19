using Couchy.Api.Enums;

namespace Couchy.Api.Entities;

public class User
{
    public long UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>BCrypt hash, never the plain password.</summary>
    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.ROLE_USER;

    public UserStatus Status { get; set; } = UserStatus.ACTIVE;

    // Filled in automatically by CouchyDbContext on save.
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Profile? Profile { get; set; }
}
