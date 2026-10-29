
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleDataSimulator.Infrastructure.ExternalServices;
using Xunit;

namespace VehicleDataSimulator.Tests;

public class VehicleApiClientTests
{
    [Fact]
    public async Task SendDataAsync_WhenApiReturnsSuccess_ReturnsTrue()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var logger = Mock.Of<ILogger<VehicleApiClient>>();
        var sut = new VehicleApiClient(httpClient, logger);

        var result = await sut.SendDataAsync(new { VehicleId = "test-vehicle" }, "34TEST123", CancellationToken.None);

        Assert.True(result);
        Assert.Equal("api/vehicle-data/34TEST123", handler.RequestUri?.PathAndQuery.TrimStart('/'));
    }

    [Fact]
    public async Task SendDataAsync_WhenApiReturnsFailure_ReturnsFalse()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var logger = Mock.Of<ILogger<VehicleApiClient>>();
        var sut = new VehicleApiClient(httpClient, logger);

        var result = await sut.SendDataAsync(
            new { VehicleId = "test-vehicle" },
            "34TEST123",
            CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task SendDataAsync_WhenRequestThrowsHttpRequestException_ReturnsFalse()
    {
        var handler = new FakeHttpMessageHandler(_ => throw new HttpRequestException("Connection failed"));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var logger = Mock.Of<ILogger<VehicleApiClient>>();
        var sut = new VehicleApiClient(httpClient, logger);

        var result = await sut.SendDataAsync(
            new { VehicleId = "test-vehicle" },
            "34TEST123",
            CancellationToken.None);

        Assert.False(result);
    }

    private sealed class FakeHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            return Task.FromResult(responseFactory(request));
        }
    }
}
