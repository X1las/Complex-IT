using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class UserRatingDataService
{
    // DB stores rating as string, API uses int
    public bool AddOrUpdateRating(int userId, string titleId, int rating)
    {
        // Validate rating range (1-10)
        if (rating < 1 || rating > 10)
        {
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(titleId))
        {
            return false;
        }
        
        using var db = new ImdbContext();
        
        // Check if rating already exists
        var existingRating = db.UsersRating.FirstOrDefault(r => 
            r.Username == userId.ToString() && r.TitleId == titleId);
        
        if (existingRating != null)
        {
            // Update existing rating
            existingRating.Rating = rating.ToString();
        }
        else
        {
            // Create new rating
            var newRating = new UserRatings
            {
                Username = userId.ToString(),
                TitleId = titleId,
                Rating = rating.ToString()
            };
            db.UsersRating.Add(newRating);
        }
        
        db.SaveChanges();
        return true;
    }
    
    // READ - Get all ratings for a user
    public (List<UserRatings> ratings, int totalCount) GetUserRatings(int userId)
    {
        using var db = new ImdbContext();
        
        var query = db.UsersRating
            .Where(r => r.Username == userId.ToString())
            .AsQueryable();
        
        var totalCount = query.Count();
        
        var ratings = query
            .OrderByDescending(r => r.TitleId)
            .ToList();
        
        return (ratings, totalCount);
    }
    
    // READ - Get specific rating for a title
    public UserRatings? GetUserRating(int userId, string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId))
        {
            return null;
        }
        
        using var db = new ImdbContext();
        return db.UsersRating
            .FirstOrDefault(r => r.Username == userId.ToString() && r.TitleId == titleId);
    }
    
    // DELETE - Remove rating
    public bool DeleteRating(int userId, string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId))
        {
            return false;
        }
        
        using var db = new ImdbContext();
        
        var rating = db.UsersRating.FirstOrDefault(r => 
            r.Username == userId.ToString() && r.TitleId == titleId);
        
        if (rating == null)
        {
            return false;
        }
        
        db.UsersRating.Remove(rating);
        db.SaveChanges();
        
        return true;
    }
    
    // DELETE - Clear all ratings for a user
    public bool ClearUserRatings(int userId)
    {
        using var db = new ImdbContext();
        
        var ratings = db.UsersRating.Where(r => r.Username == userId.ToString());
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
            .Where(r => r.TitleId == titleId && !string.IsNullOrEmpty(r.Rating))
            .ToList();

        if (!ratings.Any())
        {
            return 0;
        }

        // Convert string ratings to double and calculate average
        var validRatings = ratings
            .Select(r => double.TryParse(r.Rating, out double val) ? val : 0)
            .Where(r => r > 0)
            .ToList();

        return validRatings.Any() ? validRatings.Average() : 0;
    }
      
    // COUNT - Get total ratings count for user
    public int GetRatingCount(int userId)
    {
        using var db = new ImdbContext();
        return db.UsersRating.Count(r => r.Username == userId.ToString());
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