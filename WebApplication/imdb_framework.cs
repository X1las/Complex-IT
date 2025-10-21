// namespace DataServiceLayer;

public class Users
{
    public required string UserName { get; set; }
    public required string Pswd { get; set; }
}

public class UserRatings

{
    public required string UserName { get; set; }
    public required string TitleId { get; set; }
    public string? Rating { get; set; }
    public List<UserRatings>? UserRatingsDetails { get; set; }
}

public class UserHistory
{
    public required string UserName { get; set; }
    public DateTime Date { get; set; }
    public DateTime Required { get; set; }
    public string? TitleId { get; set; }
    public List<UserHistory>? UserHistoryDetails { get; set; }
}

public class Attends
{
    public required string TitleId { get; set; }
    public required string CrewId { get; set; }
    public string? CrewRole { get; set; }
    public string? Job { get; set; }
    public string? CrewCharacter { get; set; }
    public List<Attends>? AttendsDetails { get; set; }
}

public class AttributeAlts
{
    public required string TitleId { get; set; }
    public required string AltsOrdering { get; set; }
    public required string Attribute { get; set; }
    public List<AttributeAlts>? AttributeAltsDetails { get; set; }
}

public class Attributes
{
    public required string Attribute { get; set; }
    public List<Attribute>? AttributeDetails { get; set; }
}

public class Bookmarks
{
    public required string UserName { get; set; }
    public required string TitleId { get; set; }
    public List<Bookmarks>? BookmarksDetails { get; set; }
}

public class BoxofficeTitle
{
    public required string TitleId { get; set; }
    public required string BoxOffice { get; set; }
    public List<BoxofficeTitle>? BoxofficeTitleDetails { get; set; }
}

public class Crew
{
    public required string Id { get; set; }
    public string? FullName { get; set; }
    public string? BirthYear { get; set; }
    public string? DeathYear { get; set; }
    public List<Crew>? CrewDetails { get; set; }
}

public class DvdRelease
{
    public required string TitleId { get; set; }
    public required string Dvd { get; set; }
    public List<DvdRelease>? DvdReleaseDetails { get; set; }
}

public class Episode
{
    public required string EpisodeId { get; set; }
    public string? SeriesId { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
    public List<Episode>? EpisodeDetails { get; set; }
}

public class Genres
{
    public required string Genre { get; set; }
    public List<Genres>? GenreDetails { get; set; }
}

public class ImdbRatings
{
    public required string TitleId { get; set; }
    public double UserRating { get; set; }
    public int NumUserRatings { get; set; }
    public List<ImdbRatings>? ImdbRatingsDetails { get; set; }
}

public class MaturityRatings
{
    public required string TitleId { get; set; }
    public required string MaturityRating { get; set; }
    public List<MaturityRatings>? MaturityRatingsDetails { get; set; }
}

public class ProductionTitles
{
    public required string TitleId { get; set; }
    public required string Production { get; set; }
    public List<ProductionTitles>? ProductionTitlesDetails { get; set; }
}

public class Productions
{
    public required string Production { get; set; }
    public List<Productions>? ProductionsDetails { get; set; }
}

public class Regions
{
    public required string Region { get; set; }
    public string? Language { get; set; }
    public List<Regions>? RegionsDetails { get; set; }
}

public class Runtimes
{
    public required string TitleId { get; set; }
    public string? RunTime { get; set; }
    public List<Runtimes>? RuntimesDetails { get; set; }
    
}

public class Series
{
    public required string SeriesId { get; set; }
    public int Episodes { get; set; }
    public int Seasons { get; set; }
    public List<Series>? SeriesDetails { get; set; }
    
}

public class TitleAwards
{
    public required string TitleId { get; set; }
    public required string Awards { get; set; }
    public List<TitleAwards>? TitleAwardsDetails { get; set; }
}

public class TitleGenres
{
    public required string TitleId { get; set; }
    public required string Genre { get; set; }
    public List<TitleGenres>? TitleGenresDetails { get; set; }
}

public class TitleMaturityRatings
{
    public required string TitleId { get; set; }
    public required string MaturityRating { get; set; }
    public List<TitleMaturityRatings>? TitleMaturityRatingsDetails { get; set; }
}

public class TitlePosters
{
    public required string TitleId { get; set; }
    public required string Poster { get; set; }
    public List<TitlePosters>? TitlePostersDetails { get; set; }
}

public class TitleWebsites
{
    public required string TitleId { get; set; }
    public required string Website { get; set; }
    public List<TitleWebsites>? TitleWebsitesDetails { get; set; }
}

public class TitleRegions
{
    public required string TitleId { get; set; }
    public required string Region { get; set; }
    public List<TitleRegions>? TitleRegionsDetails { get; set; }
}

public class WordIndex
{
    public required string TitleId { get; set; }
    public required string Word { get; set; }
    public required string Field { get; set; }
    public string? Lexeme { get; set; }
    public List<WordIndex>? WordIndexDetails { get; set; }
}

public class AlternateTitles
{
    public required string TitleId { get; set; }
    public required int Ordering { get; set; }
    public required string AltsTitle { get; set; }
    public string? Types { get; set; }
    public string? IsOriginalTitle { get; set; }
    public List<AlternateTitles>? AlternateTitlesDetails { get; set; }
}

public class Titles
{
    public required string Id { get; set; }
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
    public List<Titles>? TitlesDetails { get; set; }
}

