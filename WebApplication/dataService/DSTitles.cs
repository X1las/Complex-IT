using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class DataService
{
    public Titles? GetTitleById(ImdbContext context, string titleId)
    {
        return context.Title.FirstOrDefault(t => t.Id == titleId);
    }

    public List<Titles>? GetTitlesByName(ImdbContext context, string titleName)
    {
        return context.Title
            .Where(t => t.Title != null && t.Title.Contains(titleName))
            .ToList();
    }

    public List<Titles>? GetTitlesByGenre(ImdbContext context, string genre)
    {
        var titleIds = context.TitleGenre
            .Where(tg => tg.Genre == genre)
            .Select(tg => tg.TitleId)
            .ToList();

        return context.Title
            .Where(t => titleIds.Contains(t.Id))
            .ToList();
    }

    public List<Titles>? GetTitlesByYear(ImdbContext context, string year)
    {
        return context.Title
            .Where(t => t.Year == year)
            .ToList();
    }

    public List<Titles>? GetTitleByType(ImdbContext context, string titleType)
    {
        return context.Title
            .Where(tg => tg.TitleType == titleType)
            .ToList();
    }


    public ImdbRatings? GetTitleRatingById(ImdbContext context, string titleId)
    {
        return context.ImdbRating.FirstOrDefault(tr => tr.TitleId == titleId);
    }

    public List<Crew>? GetCrewByTitleId(ImdbContext context, string titleId)
    {
        var crewIds = context.Attend
            .Where(tc => tc.TitleId == titleId)
            .Select(tc => tc.CrewId)
            .ToList();

        return context.Crew
            .Where(c => crewIds.Contains(c.CrewId))
            .ToList();
    }

    public List<Episodes>? GetEpisodesBySeriesId(ImdbContext context, string seriesId)
    {
        return context.Episodes
            .Where(e => e.SeriesId == seriesId)
            .ToList();
    }
}