using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class UserRatingDataService
{
    // DB stores rating as string, API uses int
    public bool AddOrUpdateRating(string username, string titleId, int? rating)
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
        
        if (rating == null)
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

    // AGGREGATE - Get average rating for a title from user ratings
    public (double averageRating, int totalVotes) CalculateTitleRating(string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId))
        {
            return (0, 0);
        }
        
        using var db = new ImdbContext();
        var validRatings = db.UsersRating
            .Where(r => r.TitleId == titleId && r.Rating.HasValue)
            .ToList();
        
        if (validRatings.Count == 0)
        {
            return (0, 0);
        }
        
        var average = validRatings.Average(r => r.Rating.Value);
        return (Math.Round(average, 1), validRatings.Count);
    }
    
    // UPDATE - Update title's aggregate rating in titles table
    public void UpdateTitleAggregateRating(string titleId)
    {
        if (string.IsNullOrWhiteSpace(titleId))
        {
            return;
        }
        
        using var db = new ImdbContext();
        var title = db.Title.FirstOrDefault(t => t.Id == titleId);
        
        if (title == null)
        {
            return;
        }
        
        // Get original IMDB rating from imdb_ratings table
        var imdbRating = db.ImdbRating.FirstOrDefault(r => r.TitleId == titleId);
        
        // Get user ratings average
        var (userAvgRating, userVoteCount) = CalculateTitleRating(titleId);
        
        // If imdb_ratings doesn't have baseline data, save it now from titles table
        if (imdbRating == null && title.Rating.HasValue && title.Votes.HasValue)
        {
            imdbRating = new ImdbRatings
            {
                TitleId = titleId,
                UserRating = title.Rating.Value,
                NumUserRatings = title.Votes.Value
            };
            db.ImdbRating.Add(imdbRating);
        }
        
        // Calculate combined average with proper weighted average
        if (imdbRating != null && userVoteCount > 0)
        {
            var totalVotes = imdbRating.NumUserRatings + userVoteCount;
            var weightedSum = (imdbRating.UserRating * imdbRating.NumUserRatings) + (userAvgRating * userVoteCount);
            title.Rating = Math.Round(weightedSum / totalVotes, 1);
            title.Votes = totalVotes;
        }
        else if (imdbRating != null)
        {
            title.Rating = imdbRating.UserRating;
            title.Votes = imdbRating.NumUserRatings;
        }
        else if (userVoteCount > 0)
        {
            title.Rating = userAvgRating;
            title.Votes = userVoteCount;
        }
        
        db.SaveChanges();
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