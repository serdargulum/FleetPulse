
using AdminApi.Application.Commands.Auth.Register;
using AdminApi.Application.Common.Security;
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Moq;
using Xunit;

namespace AdminApi.Tests.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private RegisterCommandHandler CreateHandler() =>
        new(_userRepository.Object, _passwordHasher.Object);

    [Fact]
    public async Task Handle_WhenInputIsValid_CreatesUserAndReturnsResult()
    {
        var command = new RegisterCommand(
            "  Test User  ",
            "  USER@Example.COM ",
            "password123");

        _userRepository
            .Setup(x => x.ExistsByEmailAsync(
                "user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasher
            .Setup(x => x.Hash("password123"))
            .Returns("hashed-password");

        User? savedUser = null;

        _userRepository
            .Setup(x => x.AddAsync(
                It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => savedUser = user)
            .Returns(Task.CompletedTask);

        var result = await CreateHandler().Handle(
            command, CancellationToken.None);

        Assert.NotNull(savedUser);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test User", result.Name);
        Assert.Equal("user@example.com", result.Email);
        Assert.Equal(result.Id, savedUser.Id);
        Assert.Equal("Test User", savedUser.Username);
        Assert.Equal("user@example.com", savedUser.Email);
        Assert.Equal("hashed-password", savedUser.PasswordHash);
        Assert.True(savedUser.IsActive);

        _passwordHasher.Verify(x => x.Hash("password123"), Times.Once);
        _userRepository.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        var command = new RegisterCommand(
            "Test User", "user@example.com", "password123");

        _userRepository
            .Setup(x => x.ExistsByEmailAsync(
                command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await CreateHandler()
                .Handle(command, CancellationToken.None));

        Assert.Equal("An account with this email already exists.", exception.Message);

        _userRepository.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _passwordHasher.Verify(
            x => x.Hash(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("", "user@example.com", "password123", "Name is required.")]
    [InlineData("   ", "user@example.com", "password123", "Name is required.")]
    [InlineData("Test User", "", "password123", "Email is required.")]
    [InlineData("Test User", "   ", "password123", "Email is required.")]
    [InlineData("Test User", "user@example.com", "", "Password is required.")]
    [InlineData("Test User", "user@example.com", "   ", "Password is required.")]
    [InlineData("Test User", "user@example.com", "short", "Password must be at least 8 characters.")]
    public async Task Handle_WhenInputIsInvalid_ThrowsArgumentException(
        string name,
        string email,
        string password,
        string expectedMessage)
    {
        var command = new RegisterCommand(name, email, password);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await CreateHandler()
                .Handle(command, CancellationToken.None));

        Assert.Equal(expectedMessage, exception.Message);

        _userRepository.Verify(
            x => x.ExistsByEmailAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _userRepository.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _passwordHasher.Verify(
            x => x.Hash(It.IsAny<string>()), Times.Never);
    }
}
