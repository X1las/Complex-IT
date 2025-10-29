namespace WebServiceLayer.Models;

// User Authentication & Profile
public class UserRegistrationDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginResponseDto
{
    public string Url { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Token { get; set; }
}

public class UserProfileDto
{
    public string? Url { get; set; }
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
}