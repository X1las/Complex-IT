using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class DSUser
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

    public List<Bookmarks> GetUserBookmarksByUsername(ImdbContext context, string username)
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
}