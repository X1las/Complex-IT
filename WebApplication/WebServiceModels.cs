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
    public string Poster { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class TitleModelShort // TitleModel DTO
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Plot { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
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

public class TitleCrewModel // TitleCrewModel DTO
{
    public string TitleId { get; set; } = string.Empty;    
    public IEnumerable <TitleCrewModel> CrewId { get; set; }
}

public class CrewTitlesModel // CrewTitlesModel DTO
{
    public IEnumerable <CrewTitlesModel> CrewTitles { get; set; }
}
public class UserHistory // UserHistory DTO
{
    public string? TitleId { get; set; }
    public DateTime Date { get; set; }  
}

public class PagedResult<T> // Page Wrapper
{
    public List<T> Items { get; set; } = new();    
    public int Total { get; set; } 
    public int Page { get; set; } 
    public int PageSize { get; set; } 
    public int NumberOfPages => (int)Math.Ceiling((double)Total / PageSize); 
    public string? Previous { get; set; } 
    public string? Next { get; set; } 
    public string? Current { get; set; } 
}

public class EpisodesModel
{
    public string EpisodessId { get; set; } = string.Empty;
    public string SeriesId { get; set; } = string.Empty;
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
    public string EpisodeTitle { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
public class BookmarkModel // BookmarkModel DTO
{
    public string Username { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
}