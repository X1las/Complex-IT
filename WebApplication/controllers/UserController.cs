using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer;

[Route("api/users")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly ImdbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserController(ImdbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // POST: api/users/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserModel model)
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

        user.pswd = _passwordHasher.HashPassword(user, model.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserByUsername), new { username = user.Username }, new { user.Username });

    }
    // GET: api/users/{username}
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

        if (user == null)
        {
            return NotFound();
        }

        return Ok(userModel);
    }


    // POST: api/users/login
    [HttpPost("login")]
    public IActionResult Login(UserLoginModel model)
    {
        var user = _dataService.GetUser(model.Username);

        if (user == null)
        {
            return BadRequest();
        }

        if (!_hashing.Verify(model.Password, user.Password, user.Salt))
        {
            return BadRequest();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
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


  // DELETE: api/users/me
    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteOwnAccount()
    {
        
        var username = User?.Identity?.Name
                       ?? User?.FindFirst(Name)?.Value
                       ?? User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized("Unable to determine authenticated user.");

        var normalized = username.Trim().ToLowerInvariant();

        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == normalized);

        //require 'confirm' in body
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();


        return NoContent("Account deleted successfully.");
    }

}