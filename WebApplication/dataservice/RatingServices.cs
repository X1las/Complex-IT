using Microsoft.EntityFrameworkCore;
namespace DataServiceLayer;

public class UserRatingDataService
{
    public bool AddOrUpdateRating(int userId, string titleId, int rating)
    {
        if (rating < 1 || rating > 10)
        {
            return false;
        }

        using var db = new ImdbContext();
        
        var existingRating = db.UsersRating.FirstOrDefault(r => 
            r.Username == userId.ToString() && r.TitleId == titleId);
        
        if (existingRating != null)
        {
            existingRating.Rating = rating.ToString();
        }
        else
        {
            var newRating = new UserRatings
            {
                Username = userId.ToString(),
                TitleId = titleId,
                Rating = rating.ToString(),
            };
            db.UsersRating.Add(newRating);
        }
        
        db.SaveChanges();
        return true;
    }
    
    public bool DeleteRating(int userId, string titleId)
    {
        using var db = new ImdbContext();
        
        var rating = db.UsersRating.FirstOrDefault(r => 
            r.Username == userId.ToString() && r.TitleId == titleId);
        
        if (rating == null) return false;
        
        db.UsersRating.Remove(rating);
        db.SaveChanges();
        
        return true;
    }
    
    public (List<UserRatings> ratings, int totalCount) GetUserRatings(
        int userId)
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
    
    public UserRatings? GetUserRating(int userId, string titleId)
    {
        using var db = new ImdbContext();
        return db.UsersRating
            .FirstOrDefault(r => r.Username == userId.ToString() && r.TitleId == titleId);
    }

    public double GetAverageUserRatingForTitle(string titleId)
    {
        using var db = new ImdbContext();
        var ratings = db.UsersRating.Where(r => r.TitleId == titleId);
        
        if (!ratings.Any()) return 0;
        
        return ratings.Average(r => double.Parse(r.Rating ?? "0"));
    }

    public int GetRatingCount(int userId)
    {
        using var db = new ImdbContext();
        return db.UsersRating.Count(r => r.Username == userId.ToString());
    }
}