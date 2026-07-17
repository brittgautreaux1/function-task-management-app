using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementApi.DTOs;
using TaskManagementApi.Entities;
using TaskManagementApi.Extensions;

namespace TaskManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest("Email already exists");

        var user = new User 
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = dto.Email, 
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow 
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "User registered successfully" });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Invalid email or password");

        var token = GenerateJwtToken(user);

        return Ok(new 
        { 
            token,
            user = new 
            { 
                Id = user.Id, 
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        });
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) 
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) 
            return NotFound();

        return Ok(new 
        { 
            id = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName
        });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}