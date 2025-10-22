using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class DataService
{
    // Users
    public Users? GetUserByUsername(ImdbContext context, string username)
    {
        return context.User.FirstOrDefault(u => u.Username == username);
    }

    public bool CreateUser(ImdbContext context, Users newUser)
    {
        try
        {
            context.User.Add(new Users { Username = newUser.Username, Pswd = newUser.Pswd });
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<UserRatings>? GetUserRatingsByUsername(ImdbContext context, string username)
    {
        return context.UsersRating
            .Where(ur => ur.Username == username)
            .ToList();
    }

    public bool CreateUserRating(ImdbContext context, UserRatings rating)
    {
        try
        {
            context.UsersRating.Add(new UserRatings { Username = rating.Username, TitleId = rating.TitleId, Rating = rating.Rating });
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<Bookmarks>? GetUserBookmarksByUsername(ImdbContext context, string username)
    {
        return context.Bookmark
            .Where(b => b.Username == username)
            .ToList();
    }

    public bool CreateUserBookmark(ImdbContext context, Bookmarks bookmark)
    {
        try
        {
            context.Bookmark.Add(new Bookmarks { Username = bookmark.Username, TitleId = bookmark.TitleId });
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<UserHistory>? GetUserHistoryByUsername(ImdbContext context, string username)
    {
        return context.UsersHistory
            .Where(uh => uh.Username == username)
            .ToList();
    }

    public bool CreateUserHistory(ImdbContext context, UserHistory history)
    {
        try
        {
            context.UsersHistory.Add(new UserHistory { Username = history.Username, TitleId = history.TitleId, Date = history.Date });
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // Titles
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

    // Crew

    public Crew? GetCrewById(ImdbContext context, string crewId)
    {
        return context.Crew.FirstOrDefault(c => c.CrewId == crewId);
    }

    public List<Crew>? GetCrewByName(ImdbContext context, string crewName)
    {

        return context.Crew
            .Where(c => c.Fullname != null && c.Fullname.Contains(crewName))
            .ToList();

    }

    public List<Titles>? GetTitlesByCrewId(ImdbContext context, string crewId)
    {
        var titleIds = context.Attend
            .Where(tc => tc.CrewId == crewId)
            .Select(tc => tc.TitleId)
            .ToList();

        return context.Title
            .Where(t => titleIds.Contains(t.Id))
            .ToList();
    }
}