using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataServiceLayer;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;


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
            return Forbid(); // 403 - user is authenticated but trying to access another user's data
        }

        return null; // Validation passed
    }

    [HttpPost("{titleId}")]
    public async Task<IActionResult> RecordHistory(string username, string titleId)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        if (string.IsNullOrWhiteSpace(titleId))
        {
            return BadRequest(new ErrorResponseDto { Error = "TitleId cannot be empty" });
        }

        try
        {
            await Task.Run(() => _historyService.RecordUserHistory(username, titleId));

            _logger.LogInformation("User {Username} recorded history for title {TitleId}", username, titleId);

            return Ok(new { message = "History recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording history for title {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while recording history" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory(string username)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        try
        {
            var (history, totalCount) = await Task.Run(() => _historyService.GetUserHistory(username));
            
            var historyDtos = history.Select(h => new HistoryItemDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/history/{h.TitleId}/{h.Date:O}",
                Id = 0,
                TitleId = h.TitleId ?? "",
                ViewedAt = h.Date
            }).ToList();
            
            return Ok(new
            {
                history = historyDtos,
                totalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user history for username {Username}", username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving history" });
        }
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentHistory(string username, [FromQuery] int limit = 10)
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
            
            var historyDtos = history.Select(h => new HistoryItemDto
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
            _logger.LogError(ex, "Error retrieving recent history for username {Username}", username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving recent history" });
        }
    }

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

    [HttpDelete("{titleId}/{timestamp}")]
    public async Task<IActionResult> DeleteHistoryItem(string username, string titleId, DateTime timestamp)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        if (string.IsNullOrWhiteSpace(titleId))
        {
            return BadRequest(new ErrorResponseDto { Error = "TitleId cannot be empty" });
        }

        try
        {
            var success = await Task.Run(() => _historyService.DeleteHistoryItem(username, titleId, timestamp));

            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "History item not found" });
            }

            _logger.LogInformation("User {Username} deleted history for title {TitleId} at {Timestamp}",
                username, titleId, timestamp);

            return Ok(new { message = "History item deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting history item for title {TitleId} at {Timestamp}", titleId, timestamp);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting history item" });
        }
    }

    [HttpDelete("{titleId}")]
    public async Task<IActionResult> DeleteTitleHistory(string username, string titleId)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;

        if (string.IsNullOrWhiteSpace(titleId))
        {
            return BadRequest(new ErrorResponseDto { Error = "TitleId cannot be empty" });
        }

        try
        {
            var success = await Task.Run(() => _historyService.DeleteTitleHistory(username, titleId));
            
            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "No history found for this title" });
            }
            
            _logger.LogInformation("User {Username} deleted all history for title {TitleId}", username, titleId);
            
            return Ok(new { message = "Title history deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting title history for {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting title history" });
        }
    }

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