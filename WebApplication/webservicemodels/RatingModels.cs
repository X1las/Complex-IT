namespace WebServiceLayer.Models;

public class RatingDto
{
    public string Url { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
    public string? TitleName { get; set; }
    public int Rating { get; set; }  // API uses int (1-10)
    public DateTime CreatedAt { get; set; }
}
public class CreateRatingDto
{
    public string TitleId { get; set; } = string.Empty;
    public int Rating { get; set; }  // 1-10
}
public class UpdateRatingDto
{
    public int Rating { get; set; }  // 1-10
}

public class AverageRatingDto
{
    public string TitleId { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
}