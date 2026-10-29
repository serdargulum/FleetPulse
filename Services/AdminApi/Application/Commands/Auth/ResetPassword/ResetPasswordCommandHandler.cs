using System.Security.Cryptography;
using AdminApi.Application.Common.Security;
using AdminApi.Application.Contracts.Repositories;
using Mediator;

namespace AdminApi.Application.Commands.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<ResetPasswordCommand>
{
    public async ValueTask<Unit> Handle(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var tokenBytes = Convert.FromBase64String(command.Token);
        var tokenHash = Convert.ToHexString(SHA256.HashData(tokenBytes));

        var user = await userRepository.GetByPasswordResetTokenHashAsync(
            tokenHash, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (user.PasswordResetTokenExpiresAtUtc is null)
        {
            throw new InvalidOperationException("Reset token expiration is null.");
        }

        if (user.PasswordResetTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Reset token has expired.");
        }

        if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters.");
        }

        user.PasswordHash = passwordHasher.Hash(command.Password);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAtUtc = null;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Unit.Value;
    }
}