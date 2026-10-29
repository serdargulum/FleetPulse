
using AdminApi.Application.Commands.Auth.ForgotPassword;
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Application.Contracts.Services;
using AdminApi.Domain.Entities;
using Moq;
using Xunit;

namespace AdminApi.Tests.Auth;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IEmailSender> _emailSender = new();

    private ForgotPasswordCommandHandler CreateHandler() =>
        new(_userRepository.Object, _emailSender.Object);

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_DoesNotSendEmail()
    {
        var command = new ForgotPasswordCommand("missing@example.com");

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await CreateHandler().Handle(command, CancellationToken.None);

        _emailSender.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserExists_SendsResetEmail()
    {
        var command = new ForgotPasswordCommand("user@example.com");

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Email = command.Email,
                Username = "testuser"
            });

        await CreateHandler().Handle(command, CancellationToken.None);

        _emailSender.Verify(
            x => x.SendAsync(
                command.Email,
                "FleetPulse - Password Reset",
                It.Is<string>(body =>
                    body.Contains("reset-password?token=")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserExists_SavesTokenHashAndExpiration()
    {
        var command = new ForgotPasswordCommand("user@example.com");
        var user = new User
        {
            Email = command.Email,
            Username = "testuser"
        };

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var before = DateTime.UtcNow;

        await CreateHandler().Handle(command, CancellationToken.None);

        var after = DateTime.UtcNow;

        Assert.False(string.IsNullOrWhiteSpace(user.PasswordResetTokenHash));
        Assert.Equal(64, user.PasswordResetTokenHash.Length);

        Assert.NotNull(user.PasswordResetTokenExpiresAtUtc);
        Assert.InRange(
            user.PasswordResetTokenExpiresAtUtc.Value,
            before.AddMinutes(30),
            after.AddMinutes(30));

        _userRepository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NormalizesEmailBeforeLookingUpUser()
    {
        var command = new ForgotPasswordCommand(" User@Example.com ");

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                "user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await CreateHandler().Handle(command, CancellationToken.None);

        _userRepository.Verify(
            x => x.GetByEmailAsync(
                "user@example.com", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
