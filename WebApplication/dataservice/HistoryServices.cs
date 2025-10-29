using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class UserHistoryDataService
{
    // Record a new viewing
    // example RecordUserHistory(1, "tt1234567");
    public void RecordUserHistory(int userId, string titleId)
    {
        using var db = new ImdbContext();

        var history = new UserHistory
        {
            Username = userId.ToString(),
            TitleId = titleId,
            Date = DateTime.UtcNow
        };

        db.UsersHistory.Add(history);
        db.SaveChanges();
    }

    // Get all history for a user with total count
    // example: GetUserHistory(1); returns all viewings for user 1
    public (List<UserHistory> history, int totalCount) GetUserHistory(int userId)
    {
        using var db = new ImdbContext();

        var query = db.UsersHistory
            .Where(h => h.Username == userId.ToString())
            .AsQueryable();

        var totalCount = query.Count();

        var history = query
            .OrderByDescending(h => h.Date)
            .ToList();

        return (history, totalCount);
    }

    // Get recent history entries with limit
    // example: GetRecentHistory(1, 10); returns the 10 most recent viewings for user 1
    public List<UserHistory> GetRecentHistory(int userId, int limit = 10)
    {
        using var db = new ImdbContext();

        return db.UsersHistory
            .Where(h => h.Username == userId.ToString())
            .OrderByDescending(h => h.Date)
            .Take(limit)
            .ToList();
    }

    // Get count of history entries
    // example: GetHistoryCount(1); returns the number of viewings for user 1
    public int GetHistoryCount(int userId)
    {
        using var db = new ImdbContext();
        return db.UsersHistory.Count(h => h.Username == userId.ToString());
    }

    // Delete a specific viewing by timestamp
    // example: DeleteHistoryItem(1, "tt1234567", someDateTime);
    public bool DeleteHistoryItem(int userId, string titleId, DateTime timestamp)
    {
        using var db = new ImdbContext();

        // Find the exact record by userId, titleId, and timestamp
        var item = db.UsersHistory.FirstOrDefault(h =>
            h.Username == userId.ToString() &&
            h.TitleId == titleId &&
            h.Date == timestamp);

        if (item == null) return false;

        db.UsersHistory.Remove(item);
        db.SaveChanges();

        return true;
    }

    // Delete all viewings of a specific title for a user
    // example: DeleteTitleHistory(1, "tt1234567");
    public bool DeleteTitleHistory(int userId, string titleId)
    {
        using var db = new ImdbContext();

        // Find all records for this user and title
        var items = db.UsersHistory
            .Where(h => h.Username == userId.ToString() && h.TitleId == titleId)
            .ToList();

        if (!items.Any()) return false;

        db.UsersHistory.RemoveRange(items);
        db.SaveChanges();

        return true;
    }

    // Clear all history for a user
    // example: ClearUserHistory(1);
    public bool ClearUserHistory(int userId)
    {
        using var db = new ImdbContext();
        
        var history = db.UsersHistory.Where(h => h.Username == userId.ToString());
        db.UsersHistory.RemoveRange(history);
        db.SaveChanges();
        
        return true;
    }
}