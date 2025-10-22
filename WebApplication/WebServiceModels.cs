using DataServiceLayer;

namespace WebServiceLayer;

// Data transfer objects (DTOs) for Web Service Layer
public class TitleModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleType { get; set; }
    public string Year { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class UserModel // UserModel DTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<Bookmarks> Bookmarks { get; set; } = new();
    public List<UserRatings> Ratings { get; set; } = new();
    public List<UserHistory> History { get; set; } = new();
}

public class CrewModel // CrewModelDetails DTO
{
    public string CrewId { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string? BirthYear { get; set; }
    public string? DeathYear { get; set; }
    public double AverageRating { get; set; }
    public string Url { get; set; } = string.Empty;
}


