using Microsoft.AspNetCore.Identity;
using ResumeAI.Core.Entities;

namespace ResumeAI.Services.Auth;

public interface IPasswordService
{
    string Hash(User user, string password);
    bool Verify(User user, string hash, string password);
}

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();
    public string Hash(User user, string password) => _hasher.HashPassword(user, password);
    public bool Verify(User user, string hash, string password) =>
        _hasher.VerifyHashedPassword(user, hash, password) != PasswordVerificationResult.Failed;
}