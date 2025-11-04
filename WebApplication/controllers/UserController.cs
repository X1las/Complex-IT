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
    private readonly ILogger<UserController> _logger;
    
    public UserController(ImdbContext context,
        UserDataService dataService,
        Hashing hashing,
        IConfiguration configuration,
        ILogger<UserController> logger)
    {
        _context = context;
        _dataService = dataService;
        _hashing = hashing;
        _configuration = configuration;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(UserCredentialsModel model)
    {        
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest(new ErrorResponseDto { Error = "Username and Password are required." });
        }
        
        var username = model.Username.Trim();
        var existingUser = await _context.Users.AnyAsync(u => u.Username == model.Username);
        
        if (existingUser)
        {
            return Conflict(new ErrorResponseDto { Error = "Username already exists." });
        }

        try
        {
            var user = new Users
            {
                Username = model.Username,
            };

            (user.HashedPassword, user.Salt) = _hashing.Hash(model.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {Username} successfully created", user.Username);
            
            return CreatedAtAction(nameof(GetUserByUsername), new { username = user.Username }, new { user.Username });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user {Username}", model.Username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while creating user" });
        }
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var authenticatedUser = User?.Identity?.Name;
        
        if (string.IsNullOrWhiteSpace(authenticatedUser))
        {
            return Unauthorized(new ErrorResponseDto { Error = "Unable to determine authenticated user." });
        }
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest(new ErrorResponseDto { Error = "Username is required." });
        }

        try
        {
            var normalized = username.Trim().ToLower();

            var userDto = await _context.Users.AsNoTracking()
                .Where(u => u.Username.ToLower() == normalized)
                .Select(u => new { u.Username })
                .FirstOrDefaultAsync();

            if (userDto == null)
            {
                return NotFound(new { error = "User not found" });
            }

            _logger.LogInformation("User {AuthenticatedUser} successfully retrieved profile for {Username}", authenticatedUser, username);
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {Username}", username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving user" });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserCredentialsModel model)
    {
        _logger.LogInformation("Login attempt for username: {Username}", model.Username);
        
        try
        {
            var user = await Task.Run(() => _dataService.GetUserByUsername(model.Username));

            if (user == null)
            {
                return BadRequest(new ErrorResponseDto { Error = "Invalid username or password" });
            }

            if (!_hashing.Verify(model.Password, user.HashedPassword, user.Salt))
            {
                return BadRequest(new ErrorResponseDto { Error = "Invalid username or password" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
            };

            var secret = _configuration.GetSection("Auth:Secret").Value 
                ?? throw new InvalidOperationException("Auth:Secret is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("User {Username} successfully logged in", user.Username);
            
            return Ok(new UserLoginResponseModel { Token = jwt });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for username: {Username}", model.Username);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred during login" });
        }
    }

    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteOwnAccount()
    {
        var authenticatedUser = User?.Identity?.Name;
        
        try
        {
            if (string.IsNullOrWhiteSpace(authenticatedUser))
            {
                return Unauthorized(new ErrorResponseDto { Error = "Unable to determine authenticated user." });
            }
            if (string.IsNullOrWhiteSpace(authenticatedUser))
            {
                return BadRequest(new ErrorResponseDto { Error = "Username is required." });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == authenticatedUser.ToLower());

            if (user == null)
            {
                return NotFound();
            }

            //require 'confirm' in body
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User account {Username} successfully deleted", authenticatedUser);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during account deletion for user: {AuthenticatedUser}", authenticatedUser);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred during account deletion" });
        }
    }

}