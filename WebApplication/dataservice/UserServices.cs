using Microsoft.EntityFrameworkCore;
using WebServiceLayer.Utils;

namespace DataServiceLayer;

public class UserDataService
{
    private readonly Hashing _hashing;

    public UserDataService(Hashing hashing)
    {
        _hashing = hashing;
    }

    // CREATE
    public Users? RegisterUser(string username, string password)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }
        
        using var db = new ImdbContext();
        
        // Check if username already exists
        var existingUser = db.Users.FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
        {
            return null;  // Username taken
        }
        
        // Create new user
        var (hashedPassword, salt) = _hashing.Hash(password);
        var users = new Users
        {
            Username = username,
            HashedPassword = hashedPassword,
            Salt = salt
        };
        
        db.Users.Add(users);
        db.SaveChanges();
        
        return users;
    }
    
    // AUTHENTICATE - Login user
    public Users? AuthenticateUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }
        
        using var db = new ImdbContext();
        
        // Find user with matching credentials
        // TODO: In production, compare hashed passwords
        var user = db.Users.FirstOrDefault(u => 
            u.Username == username && u.HashedPassword == password);
        
        return user;  // null if not found or invalid credentials
    }
    
    // READ
    public Users? GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }
        
        using var db = new ImdbContext();
        return db.Users.FirstOrDefault(u => u.Username == username);
    }
    
    // READ - Get all users with pagination
    public (List<Users> users, int totalCount) GetAllUsers(int page = 0, int pageSize = 10)
    {
        using var db = new ImdbContext();
        
        var query = db.Users.AsQueryable();
        
        // Get total count before pagination
        var totalCount = query.Count();
        
        // Apply pagination
        var users = query
            .OrderBy(u => u.Username)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
        
        return (users, totalCount);
    }
    
    // UPDATE user profile
    public Users? UpdateUser(string currentUsername, string? newUsername, string? newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentUsername))
        {
            return null;
        }
        
        using var db = new ImdbContext();
        
        var user = db.Users.FirstOrDefault(u => u.Username == currentUsername);
        if (user == null)
        {
            return null;  // User not found
        }
        
        // Update password if provided
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            (user.HashedPassword, user.Salt) = _hashing.Hash(newPassword);
        }
        
        // Update username if provided (more complex - need to update related data)
        if (!string.IsNullOrWhiteSpace(newUsername) && newUsername != currentUsername)
        {
            // Check if new username is already taken
            var usernameExists = db.Users.Any(u => u.Username == newUsername);
            if (usernameExists)
            {
                return null;  // Username already taken
            }
            
            // Update username in all related tables
            // Necessary since Username is used as a foreign key
            
            // Update user ratings
            var ratings = db.UsersRating.Where(r => r.Username == currentUsername);
            foreach (var rating in ratings)
            {
                rating.Username = newUsername;
            }
            
            // Update user history
            var history = db.UsersHistory.Where(h => h.Username == currentUsername);
            foreach (var h in history)
            {
                h.Username = newUsername;
            }
            
            // Update bookmarks
            var bookmarks = db.Bookmark.Where(b => b.Username == currentUsername);
            foreach (var bookmark in bookmarks)
            {
                bookmark.Username = newUsername;
            }
            
            // Finally update the username
            user.Username = newUsername;
        }
        
        db.SaveChanges();
        return user;
    }
    
    // DELETE - Remove user account
    public bool DeleteUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }
        
        using var db = new ImdbContext();
        
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return false;  // User not found
        }
        
        var ratings = db.UsersRating.Where(r => r.Username == username);
        db.UsersRating.RemoveRange(ratings);
        
        var history = db.UsersHistory.Where(h => h.Username == username);
        db.UsersHistory.RemoveRange(history);
        
        var bookmarks = db.Bookmark.Where(b => b.Username == username);
        db.Bookmark.RemoveRange(bookmarks);
        
        db.Users.Remove(user);
        db.SaveChanges();
        
        return true;
    }
    
    // HELPER - Check if username is available
    public bool IsUsernameAvailable(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }
        
        using var db = new ImdbContext();
        return !db.Users.Any(u => u.Username == username);
    }

    // COUNT - Get total users
    public int GetUserCount()
    {
        using var db = new ImdbContext();
        return db.Users.Count();
    }

    public void UpdateUserToken(string username, string token)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        using var db = new ImdbContext();

        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return;  // User not found
        }

        user.Token = token;
        user.LastLogin = DateTime.UtcNow;

        db.SaveChanges();
    }
    
    public bool ValidateUserToken(string username, string token)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        using var db = new ImdbContext();

        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || user.Token != token)
        {
            return false;  // User not found or token mismatch
        }

        return true;  // Token is valid
    }
}