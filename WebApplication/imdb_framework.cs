namespace DataServiceLayer;

public class Users
{
    public string Username { get; set; }
    public string Pswd { get; set; }
    public List<UserRatings>? UserRatingsDetails { get; set; }
    public List<UserHistory>? UserHistoryDetails { get; set; }
    public List<Bookmarks>? BookmarksDetails { get; set; }
}

public class UserRatings
{
    public string Username { get; set; }
    public string TitleId { get; set; }
    public string? Rating { get; set; }
}

public class UserHistory
{
    public string Username { get; set; }
    public DateTime Date { get; set; }
    public string? TitleId { get; set; }
}

public class Attends
{
    public string TitleId { get; set; }
    public string CrewId { get; set; }
    public string? CrewRole { get; set; }
    public string? Job { get; set; }
    public string? CrewCharacter { get; set; }
}

public class AttributeAlts
{
    public string TitleId { get; set; }
    public string AltsOrdering { get; set; }
    public string Attribute { get; set; }

}

public class Attributes
{
    public string Attribute { get; set; }
}

public class Bookmarks
{
    public string Username { get; set; }
    public string TitleId { get; set; }
}

public class BoxofficeTitles
{
    public string TitleId { get; set; }
    public string BoxOffice { get; set; }
}

public class Crew
{
    public string CrewId { get; set; }
    public string? Fullname { get; set; }
    public string? BirthYear { get; set; }
    public string? DeathYear { get; set; }
    public double AverageRating { get; set; }
}

public class DvdReleases
{
    public string TitleId { get; set; }
    public string Dvd { get; set; }
}

public class Episodes
{
    public string EpisodeId { get; set; }
    public string? SeriesId { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
}

public class Genres
{
    public string Genre { get; set; }
}

public class ImdbRatings
{
    public string TitleId { get; set; }
    public double UserRating { get; set; }
    public int NumUserRatings { get; set; }
}

public class MaturityRatings
{
    public string TitleId { get; set; }
    public string MaturityRating { get; set; }
}

public class ProductionTitles
{
    public string TitleId { get; set; }
    public string Production { get; set; }
}

public class Productions
{
    public string Production { get; set; }
}

public class Regions
{
    public string Region { get; set; }
    public string? Language { get; set; }
}

public class RunTimes
{
    public  string TitleId { get; set; }
    public string? RunTime { get; set; }
}

public class Series
{
    public  string SeriesId { get; set; }
    public int Episodes { get; set; }
    public int Seasons { get; set; }
}

public class TitleAwards
{
    public  string TitleId { get; set; }
    public  string Awards { get; set; }
}

public class TitleGenres
{
    public  string TitleId { get; set; }
    public  string Genre { get; set; }
}

public class TitleMaturityRatings
{
    public  string TitleId { get; set; }
    public  string MaturityRating { get; set; }
}

public class TitlePosters
{
    public  string TitleId { get; set; }
    public  string Poster { get; set; }
}

public class TitleWebsites
{
    public  string TitleId { get; set; }
    public  string Website { get; set; }
}

public class TitleRegions
{
    public  string TitleId { get; set; }
    public  string Region { get; set; }
}

public class WordIndex
{
    public  string TitleId { get; set; }
    public  string Word { get; set; }
    public  string Field { get; set; }
    public string? Lexeme { get; set; }
}

public class AlternateTitles
{
    public  string TitleId { get; set; }
    public  int Ordering { get; set; }
    public  string AltsTitle { get; set; }
    public string? Types { get; set; }
    public string? IsOriginalTitle { get; set; }
}

public class Titles
{
    public  string Id { get; set; }
    public string? Title { get; set; }
    public string? TitleType { get; set; }
    public string? Plot { get; set; }
    public string? Year { get; set; }
    public string? StartYear { get; set; }
    public string? EndYear { get; set; }
    public string? Release_Date { get; set; }
    public string? OriginalTitle { get; set; }
    public bool IsAdult { get; set; }
    public double Rating { get; set; }
    public int Votes { get; set; }
}

