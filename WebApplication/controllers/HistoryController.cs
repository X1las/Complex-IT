using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataServiceLayer;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[Authorize]
[ApiController]
[Route("api/users/{username}/history")]
public class HistoryController : ControllerBase
{
    private readonly UserHistoryDataService _historyService;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(
        UserHistoryDataService historyService,
        ILogger<HistoryController> logger)
    {
        _historyService = historyService;
        _logger = logger;
    }

    // Helper method to validate the authenticated user matches the requested username
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

        return null; // Validation passed
    }

    // POST: api/users/{username}/history/{titleId}
    [HttpPost("{titleId}")]
    public async Task<IActionResult> RecordHistory(UserHistoryDto model)
    {
        // Helper Method Used
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;

        if (string.IsNullOrWhiteSpace(model.TitleId))
        {
            return BadRequest(new ErrorResponseDto { Error = "TitleId cannot be empty" });
        }

        try
        {
            await Task.Run(() => _historyService.RecordUserHistory(model.Username, model.TitleId));

            _logger.LogInformation("User {Username} recorded history for title {TitleId}", model.Username, model.TitleId);

            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording history for title {TitleId}", model.TitleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while recording history" });
        }
    }

    // GET: api/users/{username}/history
    [HttpGet]
    public async Task<IActionResult> GetHistory(string username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {

    var validationResult = ValidateUserAccess(username);
    if (validationResult != null) return validationResult;

    try
    {
        var (history, totalCount) = await Task.Run(() => _historyService.GetUserHistory(username));
        
        // Calculate pagination
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        // Apply pagination
        var paginatedHistory = history
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var historyDtos = paginatedHistory.Select(h => new HistoryDisplayItemDto
        {
            Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/history/{h.TitleId}/{h.Date:O}",
            Id = 0,
            TitleId = h.TitleId ?? "",
            ViewedAt = h.Date
        }).ToList();

        var response = new PagedResultDto<HistoryDisplayItemDto>
        {
            Items = historyDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
        
            return Ok(response);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "Error retrieving user history for username {username}", username);
        return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving history" });
        }
    }

    // GET: api/users/{username}/history/recent
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentHistory(string username,
        [FromQuery] int limit = 10)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        if (limit <= 0)
        {
            return BadRequest(new ErrorResponseDto { Error = "Limit must be greater than 0" });
        }

        try
        {
            var history = await Task.Run(() => _historyService.GetRecentHistory(username, limit));
            
            if (history == null || history.Count == 0)
            {
                return NotFound(new ErrorResponseDto { Error = "No recent history found for the user" });
            }

            var historyDtos = history.Select(h => new HistoryDisplayItemDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/history/{h.TitleId}/{h.Date:O}",
                Id = 0,
                TitleId = h.TitleId ?? "",
                ViewedAt = h.Date
            }).ToList();
            
            return Ok(historyDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent history for username {username}", username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving recent history" });
        }
    }

    // GET: api/users/{username}/history/count
    [HttpGet("count")]
    public async Task<IActionResult> GetHistoryCount(string username)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        try
        {
            var count = await Task.Run(() => _historyService.GetHistoryCount(username));
            
            return Ok(new {count});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving history count for username {Username}", username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving history count" });
        }
    }

    // DELETE: api/users/{username}/history/{titleId}/{timestamp}
    [HttpDelete("{titleId}/{timestamp}")]
    public async Task<IActionResult> DeleteHistoryItem(UserHistoryDto model)
    {
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;

        if (string.IsNullOrWhiteSpace(model.TitleId))
        {
            return BadRequest(new ErrorResponseDto { Error = "TitleId cannot be empty" });
        }

        if (!model.Date.HasValue)
        {
            return BadRequest(new ErrorResponseDto { Error = "Date is required" });
        }

        try
        {
            var success = await Task.Run(() => _historyService.DeleteHistoryItem(model.Username, model.TitleId, model.Date.Value));

            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "History item not found" });
            }

            _logger.LogInformation("User {Username} deleted history for title {TitleId} at {Timestamp}",
                model.Username, model.TitleId, model.Date.Value);

            return Ok(new { message = "History item deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting history item for title {TitleId} at {Timestamp}", model.TitleId, model.Date);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting history item" });
        }
    }

    // DELETE: api/users/{username}/history/{titleId}
    [HttpDelete("{titleId}")]
    public async Task<IActionResult> DeleteTitleHistory(UserHistoryDto model)
    {
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;

        if (string.IsNullOrWhiteSpace(model.TitleId))
        {
            return BadRequest(new ErrorResponseDto { Error = "TitleId cannot be empty" });
        }

        try
        {
            var success = await Task.Run(() => _historyService.DeleteTitleHistory(model.Username, model.TitleId));
            
            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "No history found for this title" });
            }

            _logger.LogInformation("User {Username} deleted all history for title {TitleId}", model.Username, model.TitleId);

            return Ok(new { message = "Title history deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting title history for {TitleId}", model.TitleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting title history" });
        }
    }

    // DELETE: api/users/{username}/history
    [HttpDelete]
    public async Task<IActionResult> ClearHistory(string username)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        try
        {
            await Task.Run(() => _historyService.ClearUserHistory(username));
            
            _logger.LogInformation("User {Username} cleared their entire history", username);
            
            return Ok(new { message = "History cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing user history for username {Username}", username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while clearing history" });
        }
    }
}