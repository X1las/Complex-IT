using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class TitleDataService
{
    private readonly UserHistoryDataService _historyServices;

    public TitleDataService()
    {
        _historyServices = new UserHistoryDataService();
    }

    public (List<Titles> titles, int totalCount) GetTitles(int? userId = null)
    {
        using var db = new ImdbContext();
        
        var query = db.Title.AsQueryable();
        var totalCount = query.Count();
        
        var titles = query
            .OrderBy(t => t.Title)
            .ToList();
        
        return (titles, totalCount);
    }
    
    public Titles? GetTitle(string id, int? userId = null)
    {
        using var db = new ImdbContext();
        
        var title = db.Title
            .FirstOrDefault(t => t.Id == id);
            
        if (userId.HasValue && title != null)
        {
            _historyServices.RecordUserHistory(userId.Value, id);
        }
        
        return title;
    }
    
    public (List<Titles> results, int totalCount) SearchTitles(
        string query)
    {
        using var db = new ImdbContext();
        
        var titleQuery = db.Title
            .Where(t => t.Title!.ToLower().Contains(query.ToLower()) || 
                       (t.Plot != null && t.Plot.ToLower().Contains(query.ToLower())))
            .AsQueryable();
        
        var totalCount = titleQuery.Count();
        
        var results = titleQuery
            .OrderBy(t => t.Title)
            .ToList();
        
        return (results, totalCount);
    }
    
    public List<TitleGenres> GetTitleGenres(string titleId)
    {
        using var db = new ImdbContext();
        return db.TitleGenre
            .Where(tg => tg.TitleId == titleId)
            .ToList();
    }
    
    public List<Attends> GetTitleCast(string titleId, int limit = 10)
    {
        using var db = new ImdbContext();
        return db.Attend
            .Where(a => a.TitleId == titleId)
            .Take(limit)
            .ToList();
    }

    public (List<Titles> titles, int totalCount) GetTitlesByGenre(
        string genre)
    {
        using var db = new ImdbContext();
        
        var query = db.TitleGenre
            .Where(tg => tg.Genre == genre)
            .Join(db.Title,
                tg => tg.TitleId,
                t => t.Id,
                (tg, t) => t)
            .AsQueryable();
        
        var totalCount = query.Count();
        
        var titles = query
            .OrderBy(t => t.Title)
            .ToList();
        
        return (titles, totalCount);
    }

    public bool TitleExists(string titleId)
    {
        using var db = new ImdbContext();
        return db.Title.Any(t => t.Id == titleId);
    }

    public List<Episodes>? GetTitleEpisodes(string seriesId)
    {
        using var db = new ImdbContext();
        return db.Episodes
            .Where(te => te.SeriesId == seriesId)
            .OrderBy(te => te.SeasonNumber)
            .ThenBy(te => te.EpisodeNumber)
            .ToList();
    }
}