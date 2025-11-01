using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebServiceLayer.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebServiceLayer.Utils;

namespace WebServiceLayer;

[Authorize]
[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ImdbContext _context;
    private readonly UserDataService _dataService;
    private readonly Hashing _hashing;
    private readonly IConfiguration _configuration;
    public UserController(ImdbContext context,
        UserDataService dataService,
        Hashing hashing,
        IConfiguration configuration)
    {
        _context = context;
        _dataService = dataService;
        _hashing = hashing;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserRegistrationModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest();
        }
        var username = model.Username.Trim();
        var existingUser = await _context.Users.AnyAsync(u => u.Username == model.Username);
        if (existingUser)
            return Conflict("Username already exists.");

        var user = new Users
        {
            Username = model.Username,
        };

        (user.HashedPassword, user.Salt) = _hashing.Hash(model.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserByUsername), new { username = user.Username }, new { user.Username });
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username is required.");
        }

        var normalized = username.Trim().ToLower();

        var userDto = await _context.Users.AsNoTracking().Where(u => u.Username.ToLower() == normalized)
            .Select(u => new { u.Username }).FirstOrDefaultAsync();

        if (userDto == null)
        {
            return NotFound();
        }

        return Ok(userDto);
    }


    // POST: api/users/login
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(UserLoginModel model)
    {
        var user = _dataService.GetUserByUsername(model.Username);

        if (user == null)
        {
            return BadRequest();
        }

        if (!_hashing.Verify(model.Password, user.HashedPassword, user.Salt))
        {
            return BadRequest();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
        };

        var secret = _configuration.GetSection("Auth:Secret").Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(4),
            signingCredentials: creds
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { user.Username, token = jwt });
    }


  // DELETE: api/users/{username}
    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteOwnAccount()
    {
        
        var username = User?.Identity?.Name
                       ?? User?.FindFirst(ClaimTypes.Name)?.Value
                       ?? User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized("Unable to determine authenticated user.");

        var normalized = username.Trim().ToLowerInvariant();

        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == normalized);

        if (user == null)
            return NotFound("User not found.");

        //require 'confirm' in body
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();


        return NoContent();
    }

}