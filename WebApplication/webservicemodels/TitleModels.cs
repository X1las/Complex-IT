using System;
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

public class TitleModelShort
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleType { get; set; }
    public string Year { get; set; } = string.Empty;
    public double Rating { get; set; }
}

public class TitleEpisodesModel
{
    public string EpisodessId { get; set; } = string.Empty;
    public string SeriesId { get; set; } = string.Empty;
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
    public string EpisodeTitle { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class TitleSearchModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleType { get; set; }
    public string Year { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class TitleCrewModel // TitleCrewModel DTO
{
    public string TitleId { get; set; } = string.Empty;    
    public IEnumerable <TitleCrewModel> ? CrewId { get; set; }
}