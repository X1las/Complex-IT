namespace WebServiceLayer.Models;

public class Ratingsmodel
{
    public string? Url { get; set; }
    public string TitleId { get; set; } = string.Empty;
    public string? TitleName { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserRatings
{
    public string Username { get; set; }
    public string TitleId { get; set; }
    public double Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    
}

public class CreateRating
{
    public string TitleId { get; set; } = string.Empty;
    public int Rating { get; set; }
}

public class UpdateRating
{
    public int Rating { get; set; }
}