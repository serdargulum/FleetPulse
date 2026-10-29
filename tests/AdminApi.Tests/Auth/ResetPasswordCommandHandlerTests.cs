
using System.Security.Cryptography;
using AdminApi.Application.Commands.Auth.ResetPassword;
using AdminApi.Application.Common.Security;
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Moq;
using Xunit;

namespace AdminApi.Tests.Auth;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private ResetPasswordCommandHandler CreateHandler() =>
        new(_userRepository.Object, _passwordHasher.Object);

    [Fact]
    public async Task Handle_WhenTokenIsValid_ResetsPasswordAndClearsToken()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);
        var tokenHash = Convert.ToHexString(SHA256.HashData(tokenBytes));

        var user = new User
        {
            Email = "user@example.com",
            PasswordResetTokenHash = tokenHash,
            PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(30)
        };

        _userRepository
            .Setup(x => x.GetByPasswordResetTokenHashAsync(
                tokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher
            .Setup(x => x.Hash("NewPassword123!"))
            .Returns("new-password-hash");

        var command = new ResetPasswordCommand(token, "NewPassword123!");

        await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal("new-password-hash", user.PasswordHash);
        Assert.Null(user.PasswordResetTokenHash);
        Assert.Null(user.PasswordResetTokenExpiresAtUtc);

        _userRepository.Verify(
            x => x.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTokenWasAlreadyUsed_Throws()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);
        var tokenHash = Convert.ToHexString(SHA256.HashData(tokenBytes));

        _userRepository
            .Setup(x => x.GetByPasswordResetTokenHashAsync(
                tokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new ResetPasswordCommand(token, "NewPassword123!");

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await CreateHandler().Handle(
                command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPasswordIsTooShort_ThrowsArgumentException()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);
        var tokenHash = Convert.ToHexString(SHA256.HashData(tokenBytes));

        var user = new User
        {
            Email = "user@example.com",
            PasswordResetTokenHash = tokenHash,
            PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(30)
        };

        _userRepository
            .Setup(x => x.GetByPasswordResetTokenHashAsync(
                tokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new ResetPasswordCommand(token, "short");

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await CreateHandler().Handle(
                command, CancellationToken.None));

        _userRepository.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenPasswordIsEmptyOrWhitespace_ThrowsArgumentException(
        string password)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);
        var tokenHash = Convert.ToHexString(SHA256.HashData(tokenBytes));

        var user = new User
        {
            Email = "user@example.com",
            PasswordResetTokenHash = tokenHash,
            PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(30)
        };

        _userRepository
            .Setup(x => x.GetByPasswordResetTokenHashAsync(
                tokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new ResetPasswordCommand(token, password);

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await CreateHandler().Handle(
                command, CancellationToken.None));

        _userRepository.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
