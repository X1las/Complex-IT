using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;
public class DataService
{
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

    public Titles? GetTitleById(ImdbContext context, string titleId)
    {
        return context.Title.FirstOrDefault(t => t.Id == titleId);
    }

    public List<Titles>? GetTitlesByName(ImdbContext context, string titleName)
    {
        try
        {
            return context.Title
                .Where(t => t.Title.Contains(titleName))
                .ToList();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public List<Titles> GetTitlesByGenre(ImdbContext context, string genre)
    {
        var titleIds = context.TitleGenre
            .Where(tg => tg.Genre == genre)
            .Select(tg => tg.TitleId)
            .ToList();

        return context.Title
            .Where(t => titleIds.Contains(t.Id))
            .ToList();
    }

    public ImdbRatings? GetTitleRatingById(ImdbContext context, string titleId)
    {
        return context.ImdbRating.FirstOrDefault(tr => tr.TitleId == titleId);
    }

    public List<Crew> GetCrewByName(ImdbContext context, string crewName)
    {
        try
        {
            return context.Crew
                .Where(c => c.Fullname.Contains(crewName))
                .ToList();
        }
        catch (Exception)
        {
            return new List<Crew>();
        }
    }

    public List<Titles> GetTitlesByCrewId(ImdbContext context, string crewId)
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