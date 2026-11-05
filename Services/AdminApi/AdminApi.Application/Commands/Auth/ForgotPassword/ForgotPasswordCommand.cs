using Mediator;

namespace AdminApi.Application.Commands.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Email) : IRequest;