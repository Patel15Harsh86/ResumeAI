namespace ResumeAI.Core.DTOs;

public record RegisterRequest(string Email, string FullName, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Email, string FullName, string Role, DateTime ExpiresAt);