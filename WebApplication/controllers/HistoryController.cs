using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer;

[Route("api/users/")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ImdbContext _context;

    public UserController(ImdbContext context)
    {
        _context = context;
    }

    // GET: api/users/history
    [HttpGet("history")]
    public IActionResult GetUserHistory(string username)
    {
        var history = _context.UsersHistory
            .Where(uh => uh.Username == username)
            .ToList();

        if (history == null || !history.Any())
        {
            return NotFound();
        }

        return Ok(history);
    }
}