namespace DataServiceLayer.Models;

public class Bookmarks
{
    public string Username { get; set; }
    public string TitleId { get; set; }

}

// Request DTO (what API receives)
public class CreateBookmarkDto
{
    public string TitleId { get; set; } = string.Empty;
}

// Response DTO (what API returns)
public class BookmarkDto
{
    public string? Url { get; set; }
    public string TitleId { get; set; } = string.Empty;
}