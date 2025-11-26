namespace WebServiceLayer.Models;
public class CrewModel // CrewModelDetails DTO
{
    public string CrewId { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string? BirthYear { get; set; }
    public string? DeathYear { get; set; }
    public double? AverageRating { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class CrewTitlesModel // CrewTitlesModel DTO
{
    public string TitleId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleType { get; set; }
    public string Year { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Url { get; set; } = string.Empty;
}
