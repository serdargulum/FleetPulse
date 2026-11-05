using Mediator;

namespace AdminApi.Application.Commands.Auth.Register;

public sealed record RegisterCommand(
    string Name,
    string Email,
    string Password) : IRequest<RegisterResult>;

