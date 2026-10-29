
using AdminApi.Application.Commands.Auth.Login;
using AdminApi.Application.Common.Security;
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Moq;
using Xunit;

namespace AdminApi.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private LoginCommandHandler CreateHandler() =>
        new(_userRepository.Object, _passwordHasher.Object);

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsInvalidOperationException()
    {
        var command = new LoginCommand("user@example.com", "password");

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await CreateHandler()
                .Handle(command, CancellationToken.None).AsTask());

        Assert.Equal("Invalid email or password.", exception.Message);

        _passwordHasher.Verify(
            x => x.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsIncorrect_ThrowsInvalidOperationException()
    {
        var command = new LoginCommand("user@example.com", "wrong-password");
        var user = new User
        {
            Username = "testuser",
            Email = command.Email,
            PasswordHash = "stored-hash"
        };

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher
            .Setup(x => x.Verify(command.Password, user.PasswordHash))
            .Returns(false);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
             async () => await CreateHandler()
                 .Handle(command, CancellationToken.None).AsTask());

        Assert.Equal("Invalid email or password.", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ReturnsLoginResponse()
    {
        var command = new LoginCommand("user@example.com", "correct-password");
        var user = new User
        {
            Username = "testuser",
            Email = command.Email,
            PasswordHash = "stored-hash"
        };

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher
            .Setup(x => x.Verify(command.Password, user.PasswordHash))
            .Returns(true);

        var result = await CreateHandler().Handle(
            command, CancellationToken.None);

        Assert.Equal("testuser", result.Name);
        Assert.Equal(command.Email, result.Email);
    }
}
