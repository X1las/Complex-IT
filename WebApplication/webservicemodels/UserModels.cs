namespace WebServiceLayer.Models;

// User Authentication & Profile
public class UserRegistrationModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginResponseModel
{
    public string Url { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Token { get; set; }
}

public class UserProfileModel
{
    public string? Url { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}