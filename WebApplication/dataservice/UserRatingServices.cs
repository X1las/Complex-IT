using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class UserRatingDataService
{
    // DB stores rating as double, API uses int
    public bool AddOrUpdateRating(string username, string titleId, int rating)
    {
        // Validate rating range (1-10)
        if (rating < 1 || rating > 10)
        {
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(titleId) || string.IsNullOrWhiteSpace(username))
        {
            return false;
        }
        
        using var db = new ImdbContext();
        
        // Check if rating already exists
        var existingRating = db.UsersRating.FirstOrDefault(r => 
            r.Username == username && r.TitleId == titleId);
        
        if (existingRating != null)
        {
            // Update existing rating
            existingRating.Rating = rating;
        }
        else
        {
            // Create new rating
            var newRating = new UserRatings
            {
                Username = username,
                TitleId = titleId,
                Rating = rating
            };
            db.UsersRating.Add(newRating);
        }
        
        db.SaveChanges();
        return true;
    }
    
    // READ - Get all ratings for a user
    public (List<UserRatings> ratings, int totalCount) GetUserRatings(string username)
    {
        using var db = new ImdbContext();
        
        var query = db.UsersRating
            .Where(r => r.Username == username)
            .AsQueryable();
        
        var totalCount = query.Count();
        
        var ratings = query
            .OrderByDescending(r => r.TitleId)
            .ToList();
        
        return (ratings, totalCount);
    }
    
    // READ - Get specific rating for a title
    public UserRatings? GetUserRating(string username, string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId) || string.IsNullOrWhiteSpace(username))
        {
            return null;
        }
        
        using var db = new ImdbContext();
        return db.UsersRating
            .FirstOrDefault(r => r.Username == username && r.TitleId == titleId);
    }
    
    // DELETE - Remove rating
    public bool DeleteRating(string username, string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId) || string.IsNullOrWhiteSpace(username))
        {
            return false;
        }
        
        using var db = new ImdbContext();
        
        var rating = db.UsersRating.FirstOrDefault(r => 
            r.Username == username && r.TitleId == titleId);
        
        if (rating == null)
        {
            return false;
        }
        
        db.UsersRating.Remove(rating);
        db.SaveChanges();
        
        return true;
    }
    
    // DELETE - Clear all ratings for a user
    public bool ClearUserRatings(string username)
    {
        using var db = new ImdbContext();
        
        var ratings = db.UsersRating.Where(r => r.Username == username);
        db.UsersRating.RemoveRange(ratings);
        db.SaveChanges();
        
        return true;
    }

    // AGGREGATE - Get average rating for a title
    public double GetAverageUserRatingForTitle(string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId))
        {
            return 0;
        }

        using var db = new ImdbContext();

        var ratings = db.UsersRating
            .Where(r => r.TitleId == titleId && r.Rating > 0)
            .ToList();

        if (!ratings.Any())
        {
            return 0;
        }

        // Rating is already double, calculate average directly
        return ratings.Average(r => r.Rating);
    }
      
    // COUNT - Get total ratings count for user
    public int GetRatingCount(string username)
    {
        using var db = new ImdbContext();
        return db.UsersRating.Count(r => r.Username == username);
    }
    
    // COUNT - Get total ratings for a title
    public int GetTitleRatingCount(string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId))
        {
            return 0;
        }
        
        using var db = new ImdbContext();
        return db.UsersRating.Count(r => r.TitleId == titleId);
    }
}