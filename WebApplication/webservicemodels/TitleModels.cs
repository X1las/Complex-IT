namespace WebServiceLayer.Models;

public class TitleModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleType { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Plot { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
}

public class TitleModelShort
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleType { get; set; }
    public string Year { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Url { get; set; } = string.Empty;
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

public class TitleCrewModel
{
    public string? CrewId { get; set; }
    public string? TitleId { get; set; }
}

public class TitleGenres
{
    public string TitleId { get; set; }
    public string Genre { get; set; }
    
}