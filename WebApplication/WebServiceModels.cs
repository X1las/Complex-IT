using DataServiceLayer;

namespace WebServiceLayer;

// Data transfer objects (DTOs) for Web Service Layer
public class TitleModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleType { get; set; }
    public string Year { get; set; } = string.Empty;
    public string? Plot { get; set; } = null;
    public double Rating { get; set; }
    public string? Poster { get; set; } = null;
    public string? Url { get; set; } = null;
}

public class TitleEpisodesModel
{
    public string TitleId { get; set; } = string.Empty;
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string? EpisodeTitle { get; set; }
}

public class TitleModelShort
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Plot { get; set; } = null;
    public string? Poster { get; set; } = null;
    public string? Url { get; set; } = null;
}

public class UserModel // UserModel DTO
{
    public string Username { get; set; } = string.Empty;
}

public class BookmarksModel // BookmarksModel DTO
{
    public string Username { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
}

public class UserRatingsModel // UserRatingsModel DTO
{
    public string Username { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
    public string? Rating { get; set; }
}

public class UserHistoryModel // UserHistoryModel DTO
{
    public string Username { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? TitleId { get; set; }
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

public class CrewTitlesModel // CrewTitlesModel DTO
{
    public string CrewId { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
    public string? CrewRole { get; set; }
    public string? Job { get; set; }
    public string? CrewCharacter { get; set; }
}
