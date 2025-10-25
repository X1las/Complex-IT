using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class UserDataService
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
            context.UsersHistory.Add(new UserHistory { Username = history.Username, TitleId = history.TitleId});
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Users? RegisterUser(string username, string password)
    {
        using var db = new ImdbContext();
        
        if (db.User.Any(u => u.Username == username))
        {
            return null;
        }
        
        var user = new Users
        {
            Username = username,
            Pswd = HashPassword(password),
        };
        
        db.User.Add(user);
        db.SaveChanges();
        
        return user;
    }
    
    public Users? Login(string username, string password)
    {
        using var db = new ImdbContext();
        
        var hashedPassword = HashPassword(password);
        return db.User.FirstOrDefault(u => 
            u.Username == username && u.Pswd == hashedPassword);
    }
    
    public Users? GetUser(int userId)
    {
        using var db = new ImdbContext();
        return db.User.FirstOrDefault(u => u.Username == userId.ToString());
    }

    public Users? GetUserByUsername(string username)
    {
        using var db = new ImdbContext();
        return db.User.FirstOrDefault(u => u.Username == username);
    }
    
    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

}