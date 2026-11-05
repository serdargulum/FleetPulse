using Mediator;


namespace AdminApi.Application.Commands.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;