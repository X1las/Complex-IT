namespace WebServiceLayer.Models;

public class UserCredentialsModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginResponseModel
{
    public string? Token { get; set; }
}

// For future purposes
public class UserProfileModel
{
    public string? Url { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}