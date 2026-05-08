using api.Contracts.Auth;
using api.Data;
using api.Entities;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api.Controller;

[ApiController]
[Route("auth")]
public class AuthController(CodeCraftDbContext db, ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Username == username))
            return Conflict(new { error = "Username already exists" });

        if (await db.Users.AnyAsync(u => u.Email == email))
            return Conflict(new { error = "Email already exists" });

        var user = new User
        {
            Username = username,
            Email = email,
            Name = request.Name.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, new UserResponse(user.Username, user.Email, user.Name)));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var username = request.Username.Trim();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid username or password" });

        var token = tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, new UserResponse(user.Username, user.Email, user.Name)));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> CurrentUser()
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (username is null) return Unauthorized();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return NotFound();

        return Ok(new UserResponse(user.Username, user.Email, user.Name));
    }

    [HttpGet("user")]
    public async Task<ActionResult<UserResponse>> UserByUsername([FromQuery] string username)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return NotFound(new { error = "User not found" });

        return Ok(new UserResponse(user.Username, user.Email, user.Name));
    }

    [HttpGet("fetch")]
    [Authorize]
    public Task<ActionResult<UserResponse>> FetchCompat() => CurrentUser();
}
