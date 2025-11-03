using Microsoft.AspNetCore.Mvc;
using DataServiceLayer;
using WebServiceLayer.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebServiceLayer.Controllers;

[Authorize]
[ApiController]
[Route("api/users/{username}/ratings")]
public class RatingController : ControllerBase
{
    private readonly UserRatingDataService _ratingService;
    private readonly ILogger<RatingController> _logger;

    public RatingController(UserRatingDataService ratingService, ILogger<RatingController> logger)
    {
        _ratingService = ratingService;
        _logger = logger;
    }
    
    private string GetAuthenticatedUsername()
    {
        return User?.Identity?.Name ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    // Validate that the route username matches the authenticated user
    private bool ValidateUsername(string username)
    {
        var authenticatedUsername = GetAuthenticatedUsername();
        return username.Equals(authenticatedUsername, StringComparison.OrdinalIgnoreCase);
    }
    
    // POST /api/users/{username}/ratings
    // Create or update a rating
    [HttpPost]
    public async Task<IActionResult> CreateRating(string username, CreateRatingDto dto)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            if (string.IsNullOrWhiteSpace(dto.TitleId))
            {
                return BadRequest(new ErrorResponseDto { Error = "TitleId is required" });
            }

            if (dto.Rating < 1 || dto.Rating > 10)
            {
                return BadRequest(new ErrorResponseDto { Error = "Rating must be between 1 and 10" });
            }

            var success = await Task.Run(() => _ratingService.AddOrUpdateRating(username, dto.TitleId, dto.Rating));
            
            if (!success)
            {
                return BadRequest(new ErrorResponseDto { Error = "Failed to create rating" });
            }
            
            _logger.LogInformation("User {Username} rated title {TitleId} with {Rating}", 
                username, dto.TitleId, dto.Rating);
            
            // Get the created rating to return it
            var rating = _ratingService.GetUserRating(username, dto.TitleId);
            
            var response = new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/ratings/{dto.TitleId}",
                TitleId = dto.TitleId,
                Rating = int.TryParse(rating?.Rating, out int r) ? r : 0,
                CreatedAt = DateTime.UtcNow
            };
            
            return CreatedAtAction(nameof(GetRating), new { username, titleId = dto.TitleId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while creating rating" });
        }
    }

    // GET /api/users/{username}/ratings
    [HttpGet]
    public async Task<IActionResult> GetAllRatings(string username)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            var (ratings, totalCount) = await Task.Run(() => _ratingService.GetUserRatings(username));
            
            var ratingDtos = ratings.Select(r => new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/ratings/{r.TitleId}",
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
            _logger.LogError(ex, "Error retrieving ratings");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving ratings" });
        }
    }

    // GET /api/users/{username}/ratings/{titleId}
    // Get specific rating for a title
    [HttpGet("{titleId}")]
    public async Task<IActionResult> GetRating(string username, string titleId)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            var rating = await Task.Run(() => _ratingService.GetUserRating(username, titleId));
            
            if (rating == null)
            {
                return NotFound(new ErrorResponseDto { Error = "Rating not found" });
            }
            
            var response = new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/ratings/{rating.TitleId}",
                TitleId = rating.TitleId,
                Rating = int.TryParse(rating.Rating, out int r) ? r : 0,
                CreatedAt = DateTime.UtcNow
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rating for title {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving rating" });
        }
    }

    // GET /api/users/{username}/ratings/count
    // Get total number of ratings for user
    [HttpGet("count")]
    public async Task<IActionResult> GetRatingCount(string username)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            var count = await Task.Run(() => _ratingService.GetRatingCount(username));
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rating count");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while getting rating count" });
        }
    }

    // PUT /api/users/{username}/ratings/{titleId}
    [HttpPut("{titleId}")]
    public async Task<IActionResult> UpdateRating(string username, string titleId, [FromBody] UpdateRatingDto dto)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            if (dto.Rating < 1 || dto.Rating > 10)
            {
                return BadRequest(new ErrorResponseDto { Error = "Rating must be between 1 and 10" });
            }
            
            // Check if rating exists
            var existingRating = await Task.Run(() => _ratingService.GetUserRating(username, titleId));
            if (existingRating == null)
            {
                return NotFound(new ErrorResponseDto { Error = "Rating not found" });
            }
            
            var success = await Task.Run(() => _ratingService.AddOrUpdateRating(username, titleId, dto.Rating));
            
            if (!success)
            {
                return BadRequest(new ErrorResponseDto { Error = "Failed to update rating" });
            }
            
            _logger.LogInformation("User {Username} updated rating for title {TitleId} to {Rating}", 
                username, titleId, dto.Rating);
            
            var rating = _ratingService.GetUserRating(username, titleId);
            
            var response = new RatingDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/ratings/{rating?.TitleId}",
                TitleId = rating?.TitleId ?? "",
                Rating = int.TryParse(rating?.Rating, out int r) ? r : 0,
                CreatedAt = DateTime.UtcNow
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rating for title {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while updating rating" });
        }
    }

    // DELETE /api/users/{username}/ratings/{titleId}
    [HttpDelete("{titleId}")]
    public async Task<IActionResult> DeleteRating(string username, string titleId)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            var success = await Task.Run(() => _ratingService.DeleteRating(username, titleId));
            
            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "Rating not found" });
            }
            
            _logger.LogInformation("User {Username} deleted rating for title {TitleId}", username, titleId);
            
            return Ok(new { message = "Rating deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting rating for title {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting rating" });
        }
    }

    // DELETE /api/users/{username}/ratings
    [HttpDelete]
    public async Task<IActionResult> ClearAllRatings(string username)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            await Task.Run(() => _ratingService.ClearUserRatings(username));
            
            _logger.LogInformation("User {Username} cleared all ratings", username);
            
            return Ok(new { message = "All ratings cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing ratings");
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

    // GET /api/titles/{titleId}/ratings/average
    // Get average rating for a title across all users
    [HttpGet("average")]
    public async Task<IActionResult> GetAverageRating(string titleId)
    {
        try
        {
            var average = await Task.Run(() => _ratingService.GetAverageUserRatingForTitle(titleId));
            var count = await Task.Run(() => _ratingService.GetTitleRatingCount(titleId));
            
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