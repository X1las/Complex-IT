namespace DataServiceLayer.Models;

public class BookmarkDisplayItemDto
{
    public string Username { get; set; }
    public string TitleId { get; set; }
    public string? TitleName { get; set; }
    public string? Url { get; set; }

}

public class UserBookmarkDto
{
    public string Username { get; set; }
    public string TitleId { get; set; }
}