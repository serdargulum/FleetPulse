using Mediator;

namespace AdminApi.Application.Commands.Auth.ResetPassword;

public sealed record ResetPasswordCommand(
    string Token,
    string Password) : IRequest;