using Microsoft.AspNetCore.Mvc;
using DataServiceLayer;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[ApiController]
[Route("api/users/{userId}/ratings")]
public class RatingController : ControllerBase
{
    private readonly UserRatingDataService _ratingService;
    private readonly ILogger<RatingController> _logger;

    public RatingController(UserRatingDataService ratingService, ILogger<RatingController> logger)
    {
        _ratingService = ratingService;
        _logger = logger;
    }

    // POST /api/users/1/ratings
    // Create or update a rating
    [HttpPost]
    public IActionResult CreateRating(int userId, [FromBody] CreateRatingDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.TitleId))
            {
                return BadRequest(new ErrorResponseDto { Error = "TitleId is required" });
            }
            
            if (dto.Rating < 1 || dto.Rating > 10)
            {
                return BadRequest(new ErrorResponseDto { Error = "Rating must be between 1 and 10" });
            }
            
            var success = _ratingService.AddOrUpdateRating(userId, dto.TitleId, dto.Rating);
            
            if (!success)
            {
                return BadRequest(new ErrorResponseDto { Error = "Failed to create rating" });
            }
            
            _logger.LogInformation("User {UserId} rated title {TitleId} with {Rating}", 
                userId, dto.TitleId, dto.Rating);
            
            // Get the created rating to return it
            var rating = _ratingService.GetUserRating(userId, dto.TitleId);
            
            var response = new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{userId}/ratings/{dto.TitleId}",
                TitleId = dto.TitleId,
                Rating = int.TryParse(rating?.Rating, out int r) ? r : 0,
                CreatedAt = DateTime.UtcNow
            };
            
            return CreatedAtAction(nameof(GetRating), new { userId, titleId = dto.TitleId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating for user {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while creating rating" });
        }
    }

    // GET /api/users/1/ratings
    [HttpGet]
    public IActionResult GetAllRatings(int userId)
    {
        try
        {
            var (ratings, totalCount) = _ratingService.GetUserRatings(userId);
            
            var ratingDtos = ratings.Select(r => new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{userId}/ratings/{r.TitleId}",
                TitleId = r.TitleId,
                Rating = int.TryParse(r.Rating, out int rating) ? rating : 0,
                CreatedAt = DateTime.UtcNow
            }).ToList();
            
            return Ok(new
            {
                ratings = ratingDtos,
                totalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ratings for user {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving ratings" });
        }
    }

    // GET /api/users/1/ratings/tt0111161
    // Get specific rating for a title
    [HttpGet("{titleId}")]
    public IActionResult GetRating(int userId, string titleId)
    {
        try
        {
            var rating = _ratingService.GetUserRating(userId, titleId);
            
            if (rating == null)
            {
                return NotFound(new ErrorResponseDto { Error = "Rating not found" });
            }
            
            var response = new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{userId}/ratings/{rating.TitleId}",
                TitleId = rating.TitleId,
                Rating = int.TryParse(rating.Rating, out int r) ? r : 0,
                CreatedAt = DateTime.UtcNow
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rating for user {UserId}, title {TitleId}", userId, titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving rating" });
        }
    }

    // GET /api/users/1/ratings/count
    // Get total number of ratings for user
    [HttpGet("count")]
    public IActionResult GetRatingCount(int userId)
    {
        try
        {
            var count = _ratingService.GetRatingCount(userId);
            return Ok(new {count});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rating count for user {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while getting rating count" });
        }
    }

    // PUT /api/users/1/ratings/tt0111161
    [HttpPut("{titleId}")]
    public IActionResult UpdateRating(int userId, string titleId, [FromBody] UpdateRatingDto dto)
    {
        try
        {
            if (dto.Rating < 1 || dto.Rating > 10)
            {
                return BadRequest(new ErrorResponseDto { Error = "Rating must be between 1 and 10" });
            }
            
            // Check if rating exists
            var existingRating = _ratingService.GetUserRating(userId, titleId);
            if (existingRating == null)
            {
                return NotFound(new ErrorResponseDto { Error = "Rating not found" });
            }
            
            var success = _ratingService.AddOrUpdateRating(userId, titleId, dto.Rating);
            
            if (!success)
            {
                return BadRequest(new ErrorResponseDto { Error = "Failed to update rating" });
            }
            
            _logger.LogInformation("User {UserId} updated rating for title {TitleId} to {Rating}", 
                userId, titleId, dto.Rating);
            
            var rating = _ratingService.GetUserRating(userId, titleId);
            
            var response = new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{userId}/ratings/{rating?.TitleId}",
                TitleId = rating?.TitleId ?? "",
                Rating = int.TryParse(rating?.Rating, out int r) ? r : 0,
                CreatedAt = DateTime.UtcNow
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rating for user {UserId}, title {TitleId}", userId, titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while updating rating" });
        }
    }

    // DELETE /api/users/1/ratings/tt0111161
    [HttpDelete("{titleId}")]
    public IActionResult DeleteRating(int userId, string titleId)
    {
        try
        {
            var success = _ratingService.DeleteRating(userId, titleId);
            
            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "Rating not found" });
            }
            
            _logger.LogInformation("User {UserId} deleted rating for title {TitleId}", userId, titleId);
            
            return Ok(new { message = "Rating deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting rating for user {UserId}, title {TitleId}", userId, titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting rating" });
        }
    }

    // DELETE /api/users/1/ratings
    [HttpDelete]
    public IActionResult ClearAllRatings(int userId)
    {
        try
        {
            _ratingService.ClearUserRatings(userId);
            
            _logger.LogInformation("User {UserId} cleared all ratings", userId);
            
            return Ok(new { message = "All ratings cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing ratings for user {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while clearing ratings" });
        }
    }
}

[ApiController]
[Route("api/titles/{titleId}/ratings")]
public class TitleRatingController : ControllerBase
{
    private readonly UserRatingDataService _ratingService;
    private readonly ILogger<TitleRatingController> _logger;

    public TitleRatingController(UserRatingDataService ratingService, ILogger<TitleRatingController> logger)
    {
        _ratingService = ratingService;
        _logger = logger;
    }

    // GET /api/titles/tt0111161/ratings/average
    // Get average rating for a title across all users
    [HttpGet("average")]
    public IActionResult GetAverageRating(string titleId)
    {
        try
        {
            var average = _ratingService.GetAverageUserRatingForTitle(titleId);
            var count = _ratingService.GetTitleRatingCount(titleId);
            
            var response = new AverageRatingDto
            {
                TitleId = titleId,
                AverageRating = Math.Round(average, 2),
                TotalRatings = count
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting average rating for title {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while getting average rating" });
        }
    }
}