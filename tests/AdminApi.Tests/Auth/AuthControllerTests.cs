
using System.Security.Claims;
using AdminApi.Application.Commands.Auth.Login;
using AdminApi.Controllers;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AdminApi.Tests.Auth;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WhenCredentialsAreValid_SignsInUser()
    {
        var mediator = new Mock<IMediator>();

        mediator
            .Setup(x => x.Send(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResponse(
                "Test User",
                "user@example.com"));

        var controller = new AuthController(mediator.Object);

        var httpContext = new DefaultHttpContext();

        var services = new ServiceCollection();
        services.AddLogging();
        services
            .AddAuthentication("Cookies")
            .AddCookie("Cookies");

        httpContext.RequestServices = services.BuildServiceProvider();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = await controller.Login(
            new LoginCommand("user@example.com", "password123"),
            CancellationToken.None);

        Assert.IsType<OkObjectResult>(result);

        var setCookieHeader = httpContext.Response.Headers.SetCookie.ToString();

        Assert.Contains(".AspNetCore.Cookies", setCookieHeader);
        Assert.Contains(".AspNetCore.Cookies", httpContext.Response.Headers.SetCookie.ToString());

        mediator.Verify(
            x => x.Send(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        mediator.Verify(
            x => x.Send(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
