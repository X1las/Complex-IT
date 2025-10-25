using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class UserHistoryDataService
{
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
    
    public (List<UserHistory> history, int totalCount) GetUserHistory(
        int userId)
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

    public bool ClearUserHistory(int userId)
    {
        using var db = new ImdbContext();
        
        var history = db.UsersHistory.Where(h => h.Username == userId.ToString());
        db.UsersHistory.RemoveRange(history);
        db.SaveChanges();
        
        return true;
    }

    public bool DeleteHistoryItem(int historyId, int userId)
    {
        using var db = new ImdbContext();
        
        var item = db.UsersHistory.FirstOrDefault(h => 
            h.TitleId == historyId.ToString() && h.Username == userId.ToString());
        
        if (item == null) return false;
        
        db.UsersHistory.Remove(item);
        db.SaveChanges();
        
        return true;
    }

    public int GetHistoryCount(int userId)
    {
        using var db = new ImdbContext();
        return db.UsersHistory.Count(h => h.Username == userId.ToString());
    }

    public List<UserHistory> GetRecentHistory(int userId, int limit = 10)
    {
        using var db = new ImdbContext();
        
        return db.UsersHistory
            .Where(h => h.Username == userId.ToString())
            .OrderByDescending(h => h.Date)
            .Take(limit)
            .ToList();
    }
}