using Microsoft.EntityFrameworkCore;
namespace DataServiceLayer;

public class BookmarkDataService
{
    public bool AddBookmark(string username, string titleId)
    {
        using var db = new ImdbContext();
        
        // Check if bookmark already exists
        if (db.Bookmark.Any(b => b.Username == username && b.TitleId == titleId))
        {
            return false;
        }
        
        var bookmark = new Bookmarks
        {
            Username = username,
            TitleId = titleId
        };
        
        db.Bookmark.Add(bookmark);
        db.SaveChanges();
        
        return true;
    }
    
    public bool RemoveBookmark(string username, string titleId)
    {
        using var db = new ImdbContext();
        
        var bookmark = db.Bookmark.FirstOrDefault(b => 
            b.Username == username && b.TitleId == titleId);
        
        if (bookmark == null) return false;
        
        db.Bookmark.Remove(bookmark);
        db.SaveChanges();
        
        return true;
    }
    
    public (List<Bookmarks> bookmarks, int totalCount) GetUserBookmarks(string username)
    {
        using var db = new ImdbContext();
        
        var query = db.Bookmark
            .Where(b => b.Username == username);
        
        var totalCount = query.Count();
        
        var bookmarks = query
            .OrderBy(b => b.TitleId)
            .ToList();
        
        return (bookmarks, totalCount);
    }

    public Bookmarks? GetBookmark(string username, string titleId)
    {
        using var db = new ImdbContext();
        return db.Bookmark
            .FirstOrDefault(b => b.Username == username && b.TitleId == titleId);
    }

    public bool BookmarkExists(string username, string titleId)
    {
        using var db = new ImdbContext();
        return db.Bookmark.Any(b => b.Username == username && b.TitleId == titleId);
    }

    public int GetBookmarkCount(string username)
    {
        using var db = new ImdbContext();
        return db.Bookmark.Count(b => b.Username == username);
    }
}