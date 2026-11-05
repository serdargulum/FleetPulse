using System.Security.Cryptography;
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Application.Contracts.Services;
using Mediator;

namespace AdminApi.Application.Commands.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IEmailSender emailSender)
    : IRequestHandler<ForgotPasswordCommand>
{
    public async ValueTask<Unit> Handle(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(
            email, cancellationToken);

        if (user is null)
            return Unit.Value;

        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);
        var tokenHash = Convert.ToHexString(SHA256.HashData(tokenBytes));

        user.PasswordResetTokenHash = tokenHash;
        user.PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(30);

        await userRepository.UpdateAsync(user, cancellationToken);

        var resetLink =
            $"http://localhost:3000/reset-password?token={Uri.EscapeDataString(token)}";

        await emailSender.SendAsync(
            user.Email,
            "FleetPulse - Password Reset",
            $"Password reset link:\n\n{resetLink}\n\nThis link expires in 30 minutes.",
            cancellationToken);

        return Unit.Value;
    }
}