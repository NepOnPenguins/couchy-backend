namespace Couchy.Api.Auth;

/// <summary>
/// BCrypt, like Spring's BCryptPasswordEncoder, so existing password hashes keep working.
/// </summary>
public static class PasswordHasher
{
    public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public static bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // The stored value is not a BCrypt hash (for example an old plain-text password).
            return false;
        }
    }
}
