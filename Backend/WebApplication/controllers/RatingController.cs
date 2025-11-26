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
    
    // Validation Helpter Method
    private IActionResult? ValidateUserAccess(string requestedUsername)
    {
        var authenticatedUsername = User?.Identity?.Name;
        
        if (string.IsNullOrWhiteSpace(authenticatedUsername))
        {
            return Unauthorized(new ErrorResponseDto { Error = "Unable to determine authenticated user" });
        }

        if (authenticatedUsername != requestedUsername)
        {
            return Forbid(); // Sende a 403
        }

        return null;
    }
    
    // POST /api/users/{username}/ratings
    [HttpPost]
    public async Task<IActionResult> CreateRating(UserRatingDto model)
    {
        // Helper Method Used
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;
        
        try
        {
            if (string.IsNullOrWhiteSpace(model.TitleId))
            {
                return BadRequest(new ErrorResponseDto { Error = "TitleId is required" });
            }

            if (model.Rating < 1 || model.Rating > 10)
            {
                return BadRequest(new ErrorResponseDto { Error = "Rating must be between 1 and 10" });
            }

            if (model.Rating == null)
            {
                return BadRequest(new ErrorResponseDto { Error = "Rating is required" });
            }

            await Task.Run(() => _ratingService.AddOrUpdateRating(model.Username, model.TitleId, model.Rating));

            _logger.LogInformation("User {Username} rated title {TitleId} with {Rating}",
                model.Username, model.TitleId, model.Rating);

            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while creating rating" });
        }
    }

    // GET /api/users/{username}/ratings
    [HttpGet]
    public async Task<IActionResult> GetAllRatings(string username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;
            
        try
        {
            var (ratings, totalCount) = await Task.Run(() => _ratingService.GetUserRatings(username));
        
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var paginatedRatings = ratings
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            var ratingDtos = paginatedRatings.Select(r => new RatingDisplayItemDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/ratings/{r.TitleId}",
                TitleId = r.TitleId,
                Rating = r.Rating
                
            }).ToList();
            
            var response = new PagedResultDto<RatingDisplayItemDto>
            {
                Items = ratingDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = totalPages
            };
        
            return Ok(response);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "Error retrieving ratings");
        return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving ratings" });
        }
    }

    // GET /api/users/{username}/ratings/{titleId}
    [HttpGet("{titleId}")]
    public async Task<IActionResult> GetRating(string username, string titleId)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        try
        {
            var rating = await Task.Run(() => _ratingService.GetUserRating(username, titleId));
            
            if (rating == null)
            {
                return NotFound(new ErrorResponseDto { Error = "Rating not found" });
            }
            
            var response = new RatingDisplayItemDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/ratings/{rating.TitleId}",
                TitleId = rating.TitleId,
                Rating = rating.Rating
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
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        try
        {
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
    public async Task<IActionResult> UpdateRating(UserRatingDto model)
    {
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;

        try
        {
            if (model.Rating < 1 || model.Rating > 10)
            {
                return BadRequest(new ErrorResponseDto { Error = "New rating must be between 1 and 10" });
            }

            // Check if rating exists
            var existingRating = await Task.Run(() => _ratingService.GetUserRating(model.Username, model.TitleId));
            
            if (existingRating == null)
            {
                return NotFound(new ErrorResponseDto { Error = "Old rating not found" });
            }

            var success = await Task.Run(() => _ratingService.AddOrUpdateRating(model.Username, model.TitleId, model.Rating));
            if (!success)
            {
                return BadRequest(new ErrorResponseDto { Error = "Failed to update rating" });
            }
            
            _logger.LogInformation("User {Username} updated rating for title {TitleId} to {Rating}", 
                model.Username, model.TitleId, model.Rating);
            
            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rating for title {TitleId}", model.TitleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while updating rating" });
        }
    }

    // DELETE /api/users/{username}/ratings/{titleId}
    [HttpDelete("{titleId}")]
    public async Task<IActionResult> DeleteRating(UserRatingDto model)
    {
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;
        try
        {
            var success = await Task.Run(() => _ratingService.DeleteRating(model.Username, model.TitleId));

            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "No rating to delete" });
            }

            _logger.LogInformation("User {Username} deleted rating for title {TitleId}", model.Username, model.TitleId);

            return Ok(new { message = "Rating deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting rating for title {TitleId}", model.TitleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting rating" });
        }
    }

    // DELETE /api/users/{username}/ratings
    [HttpDelete]
    public async Task<IActionResult> ClearAllRatings(string username)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        try
        {
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