namespace Couchy.Api.Dtos;

public class RegisterRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class AuthenticationRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class AuthenticationResponse
{
    public string Token { get; set; } = string.Empty;
}
