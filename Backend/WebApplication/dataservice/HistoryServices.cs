using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class UserHistoryDataService
{
    // Record a new viewing
    // example RecordUserHistory("alice", "tt1234567");
    public void RecordUserHistory(string? username, string titleId)
    {
        using var db = new ImdbContext();

        var history = new UserHistory
        {
            Username = username,
            TitleId = titleId,
            Date = DateTime.UtcNow
        };

        db.UsersHistory.Add(history);
        db.SaveChanges();
    }

    // Get all history for a user with total count
    // example: GetUserHistory("alice"); returns all viewings for user alice
    public (List<UserHistory> history, int totalCount) GetUserHistory(string? username)
    {
        using var db = new ImdbContext();

        var query = db.UsersHistory
            .Where(h => h.Username == username)
            .AsQueryable();

        var totalCount = query.Count();

        var history = query
            .OrderByDescending(h => h.Date)
            .ToList();

        return (history, totalCount);
    }

    // Get recent history entries with limit
    // example: GetRecentHistory("alice", 10); returns the 10 most recent viewings for user alice
    public List<UserHistory> GetRecentHistory(string? username, int limit = 10)
    {
        using var db = new ImdbContext();

        return db.UsersHistory
            .Where(h => h.Username == username)
            .OrderByDescending(h => h.Date)
            .Take(limit)
            .ToList();
    }

    // Get count of history entries
    // example: GetHistoryCount("alice"); returns the number of viewings for user alice
    public int GetHistoryCount(string? username)
    {
        using var db = new ImdbContext();
        return db.UsersHistory.Count(h => h.Username == username);
    }

    // Delete a specific viewing by timestamp
    // example: DeleteHistoryItem("alice", "tt1234567", someDateTime);
    public bool DeleteHistoryItem(string? username, string titleId, DateTime timestamp)
    {
        using var db = new ImdbContext();

        // Find the exact record by username, titleId, and timestamp
        var item = db.UsersHistory.FirstOrDefault(h =>
            h.Username == username &&
            h.TitleId == titleId &&
            h.Date == timestamp);

        if (item == null) return false;

        db.UsersHistory.Remove(item);
        db.SaveChanges();

        return true;
    }

    // Delete all viewings of a specific title for a user
    // example: DeleteTitleHistory("alice", "tt1234567");
    public bool DeleteTitleHistory(string? username, string titleId)
    {
        using var db = new ImdbContext();

        // Find all records for this user and title
        var items = db.UsersHistory
            .Where(h => h.Username == username && h.TitleId == titleId)
            .ToList();

        if (!items.Any()) return false;

        db.UsersHistory.RemoveRange(items);
        db.SaveChanges();

        return true;
    }

    // Clear all history for a user
    // example: ClearUserHistory("alice");
    public bool ClearUserHistory(string? username)
    {
        using var db = new ImdbContext();

        var history = db.UsersHistory.Where(h => h.Username == username);
        db.UsersHistory.RemoveRange(history);
        db.SaveChanges();
        
        return true;
    }
}