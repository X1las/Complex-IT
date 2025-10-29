using Microsoft.AspNetCore.Mvc;
using DataServiceLayer;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

// example: api/users/1/history
[ApiController]
[Route("api/users/{userId}/history")]
public class HistoryController : ControllerBase
{
    private readonly UserHistoryDataService _historyService;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(UserHistoryDataService historyService, ILogger<HistoryController> logger)
    {
        _historyService = historyService;
        _logger = logger;
    }

    // example: POST /api/users/1/history/tt0111161
    [HttpPost("{titleId}")]
    public IActionResult RecordHistory(int userId, string titleId)
    {
        try
        {
            _historyService.RecordUserHistory(userId, titleId);
            
            _logger.LogInformation("User {UserId} recorded history for title {TitleId}", userId, titleId);
            
            return Ok(new { message = "History recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording history for title {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while recording history" });
        }
    }

    // example: GET /api/users/1/history
    [HttpGet]
    public IActionResult GetHistory(int userId)
    {
        try
        {
            var (history, totalCount) = _historyService.GetUserHistory(userId);
            
            var historyDtos = history.Select(h => new HistoryItemDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{userId}/history/{h.TitleId}/{h.Date:O}",
                Id = 0,  // Not used
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
            _logger.LogError(ex, "Error retrieving user history for userId {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving history" });
        }
    }

    // example: GET /api/users/1/history/recent?limit=10
    [HttpGet("recent")]
    public IActionResult GetRecentHistory(int userId, [FromQuery] int limit = 10)
    {
        try
        {
            var history = _historyService.GetRecentHistory(userId, limit);
            
            var historyDtos = history.Select(h => new HistoryItemDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{userId}/history/{h.TitleId}/{h.Date:O}",
                Id = 0,
                TitleId = h.TitleId ?? "",
                ViewedAt = h.Date
            }).ToList();
            
            return Ok(historyDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent history for userId {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving recent history" });
        }
    }

    // example: GET /api/users/1/history/count
    [HttpGet("count")]
    public IActionResult GetHistoryCount(int userId)
    {
        try
        {
            var count = _historyService.GetHistoryCount(userId);
            
            return Ok(new {count});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving history count for userId {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving history count" });
        }
    }

    // example: DELETE /api/users/1/history/tt0111161/2025-10-29T12:42:00.0000000Z
    [HttpDelete("{titleId}/{timestamp}")]
    public IActionResult DeleteHistoryItem(int userId, string titleId, DateTime timestamp)
    {
        try
        {
            var success = _historyService.DeleteHistoryItem(userId, titleId, timestamp);

            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "History item not found" });
            }

            _logger.LogInformation("User {UserId} deleted history for title {TitleId} at {Timestamp}",
                userId, titleId, timestamp);

            return Ok(new { message = "History item deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting history item for title {TitleId} at {Timestamp}", titleId, timestamp);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting history item" });
        }
    }

    // example: DELETE /api/users/1/history/tt0111161
    [HttpDelete("{titleId}")]
    public IActionResult DeleteTitleHistory(int userId, string titleId)
    {
        try
        {
            var success = _historyService.DeleteTitleHistory(userId, titleId);
            
            if (!success)
            {
                return NotFound(new ErrorResponseDto { Error = "No history found for this title" });
            }
            
            _logger.LogInformation("User {UserId} deleted all history for title {TitleId}", userId, titleId);
            
            return Ok(new { message = "Title history deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting title history for {TitleId}", titleId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while deleting title history" });
        }
    }

    // example: DELETE /api/users/1/history
    [HttpDelete]
    public IActionResult ClearHistory(int userId)
    {
        try
        {
            _historyService.ClearUserHistory(userId);
            
            _logger.LogInformation("User {UserId} cleared their entire history", userId);
            
            return Ok(new { message = "History cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing user history for userId {UserId}", userId);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while clearing history" });
        }
    }
}