using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class TitleDataService
{
    private readonly UserHistoryDataService _historyServices;

    public TitleDataService()
    {
        _historyServices = new UserHistoryDataService();
    }

    public (List<Titles> titles, int totalCount) GetTitles()
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
    public (List<Titles> titles, int totalCount) SearchTitles(
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
    
    public List<Attends> GetTitleCrew(string titleId)
    {
        using var db = new ImdbContext();
        return db.Attend
            .Where(a => a.TitleId == titleId)
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
    public List<Episodes>? GetTitleEpisodes(string seriesId)
    {
        using var db = new ImdbContext();
        return db.Episodes
            .Where(te => te.SeriesId == seriesId)
            .OrderBy(te => te.SeasonNumber)
            .ThenBy(te => te.EpisodeNumber)
            .ToList();
    }

    public List<string> GetAllGenres()
    {
        using var db = new ImdbContext();
        return db.TitleGenre
            .Select(tg => tg.Genre!)
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    public TitlePosters? GetTitlePoster(string titleId)
    {
        using var db = new ImdbContext();
        return db.TitlePoster
            .FirstOrDefault(tp => tp.TitleId == titleId);
    }

    public TitleWebsites? GetTitleWebsite(string titleId)
    {
        using var db = new ImdbContext();
        return db.TitleWebsite
            .FirstOrDefault(tw => tw.TitleId == titleId);
    }

    public Runtimes? GetTitleRuntime(string titleId)
    {
        using var db = new ImdbContext();
        return db.Runtime
            .FirstOrDefault(tr => tr.TitleId == titleId);
    }

    public List<TitleRegions>? GetTitleRegions(string titleId)
    {
        using var db = new ImdbContext();
        return db.TitleRegion
            .Where(tr => tr.TitleId == titleId)
            .ToList();
    }

    public Regions? GetTitleRegionDetails(string region)
    {
        using var db = new ImdbContext();
        return db.Region
            .FirstOrDefault(r => r.Region == region);
    }

    public List<AlternateTitles>? GetTitleAlternates(string titleId)
    {
        using var db = new ImdbContext();
        return db.AlternateTitle
            .Where(ta => ta.TitleId == titleId)
            .ToList();
    }
}