using System;
using DataServiceLayer;

namespace WebServiceLayer;
public class CrewModel // CrewModelDetails DTO
{
    public string CrewId { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string? BirthYear { get; set; }
    public string? DeathYear { get; set; }
    public double AverageRating { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class TitleCrewModel // TitleCrewModel DTO
{
    public string TitleId { get; set; } = string.Empty;    
    public IEnumerable <TitleCrewModel> CrewId { get; set; }
}

public class CrewTitlesModel // CrewTitlesModel DTO
{
    public IEnumerable <CrewTitlesModel> CrewTitles { get; set; }
}