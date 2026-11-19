namespace Couchy.Api.Auth;

/// <summary>Bound from the "Jwt" section of appsettings.json.</summary>
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 24;

    // The Spring project decoded its secret as Base64 (jjwt Decoders.BASE64).
    // Doing the same here means tokens issued by either backend are valid in the other.
    public byte[] GetKeyBytes() => Convert.FromBase64String(Secret);
}
