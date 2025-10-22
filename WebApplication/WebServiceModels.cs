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

public class TitleDetails // TitleDetails DTO
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleType { get; set; }
    public string? Plot { get; set; }
    public string Year { get; set; } = string.Empty;
    public string StartYear { get; set; } = string.Empty;
    public string? EndYear { get; set; }
    public string? Release_Date { get; set; }
    public string? OriginalTitle { get; set; }
    public bool IsAdult { get; set; }
    public double Rating { get; set; }
    public int Votes { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class UserModel // UserModel DTO
{
    public string Username { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class CreateUser // CreateUser DTO
{
    public string Username { get; set; } = string.Empty;
    public string Pswd { get; set; } = string.Empty;
    public List<UserRatings>? UserRatings { get; set; } = new List<UserRatings>();
    public List<UserHistory>? UserHistory { get; set; } = new List<UserHistory>();
    public List<Bookmarks>? Bookmarks { get; set; } = new List<Bookmarks>();
}

public class UserLogin // UserLogin DTO
{
    public string Username { get; set; } = string.Empty;
    public string Pswd { get; set; } = string.Empty;
}

public class CrewModelDetails // CrewModelDetails DTO
{
    public string CrewId { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string? BirthYear { get; set; }
    public string? DeathYear { get; set; }
    public double AverageRating { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class BookmarkModel
{
    public string Username { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
    public string TitleName { get; set; } = string.Empty;
    public string? Rating { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class CreateBookmark //CreateBookmarks DTO
{
    public string TitleId { get; set; } = string.Empty;
    public string Username {get; set;} = string.Empty;
}
