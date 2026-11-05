using AdminApi.Application.Common.Security;
using AdminApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AdminApi.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<User> _hasher;

    public PasswordHasher(Microsoft.AspNetCore.Identity.IPasswordHasher<User> hasher)
    {
        _hasher = hasher;
    }

    public string Hash(string password)
    {
        var user = new User();

        return _hasher.HashPassword(user, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var user = new User();

        var result = _hasher.VerifyHashedPassword(
            user,
            passwordHash,
            password);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}