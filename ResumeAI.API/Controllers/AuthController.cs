using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeAI.Core.DTOs;
using ResumeAI.Core.Entities;
using ResumeAI.Infrastructure.Data;
using ResumeAI.Services.Auth;


namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthController(AppDbContext db, IPasswordService passwordService, IJwtService jwtService)
    {
        _db = db;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already registered.");

        var user = new User { Email = request.Email, FullName = request.FullName };
        user.PasswordHash = _passwordService.Hash(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var (token, expiry) = _jwtService.GenerateToken(user);
        return Ok(new AuthResponse(token, user.Email, user.FullName, user.Role, expiry));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !_passwordService.Verify(user, user.PasswordHash, request.Password))
            return Unauthorized("Invalid email or password.");

        var (token, expiry) = _jwtService.GenerateToken(user);
        return Ok(new AuthResponse(token, user.Email, user.FullName, user.Role, expiry));
    }
}