namespace WebServiceLayer.Models;

public class RatingDisplayItemDto
{
    public string Url { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
    public string? TitleName { get; set; }
    public double? AverageRating { get; set; }
    public int? Rating { get; set; }
}
public class UserRatingDto
{
    public string Username { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
    public int? Rating { get; set; }
}