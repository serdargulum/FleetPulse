using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleGateway.Infrastructure.Middleware;
using Xunit;

namespace VehicleGateway.Tests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_InvalidOperationException_Returns409()
    {
        var middleware = CreateMiddleware(
            _ => throw new InvalidOperationException("Conflict"));

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);

        var body = await ReadResponseBody(context);
        Assert.Equal("Conflict", body.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_Returns400()
    {
        var middleware = CreateMiddleware(
            _ => throw new ValidationException("Invalid data"));

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var body = await ReadResponseBody(context);
        Assert.Equal("Invalid data", body.RootElement.GetProperty("error").GetString());
    }

    [Fact]
    public async Task InvokeAsync_UnexpectedException_Returns500()
    {
        var middleware = CreateMiddleware(
            _ => throw new Exception("Unexpected"));

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        var body = await ReadResponseBody(context);
        Assert.Equal(
            "Unexpected An unexpected error occurred.",
            body.RootElement.GetProperty("message").GetString());
    }

    private static ExceptionHandlingMiddleware CreateMiddleware(
        RequestDelegate next)
    {
        var logger = Mock.Of<ILogger<ExceptionHandlingMiddleware>>();
        return new ExceptionHandlingMiddleware(next, logger);
    }

    private static async Task<JsonDocument> ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        return await JsonDocument.ParseAsync(context.Response.Body);
    }
}