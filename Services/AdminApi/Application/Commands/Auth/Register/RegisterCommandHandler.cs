
using AdminApi.Application.Common.Security;
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Mediator;

namespace AdminApi.Application.Commands.Auth.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async ValueTask<RegisterResult> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var name = command.Name.Trim();
        var email = command.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(command.Password))
            throw new ArgumentException("Password is required.");

        if (command.Password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters.");

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new InvalidOperationException(
                "An account with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = name,
            Email = email,
            PasswordHash = passwordHasher.Hash(command.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user, cancellationToken);

        return new RegisterResult(user.Id, user.Username, user.Email);
    }
}

public sealed record RegisterResult(
    Guid Id,
    string Name,
    string Email);
