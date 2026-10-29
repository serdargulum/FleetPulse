
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using VehicleGateway.Options;
using System.Net.Http.Json;

namespace VehicleGateway.Tests;

public class VehicleGatewayHttpTests
{
    [Fact]
    public async Task PostVehicleData_PublishesMessageToRabbitMq()
    {
        var password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? throw new InvalidOperationException("RABBITMQ_PASS ortam değişkeni tanımlanmalı.");

        var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? throw new InvalidOperationException("RABBITMQ_USER ortam değişkeni tanımlanmalı.");

        var queueName = $"test-vehicle-data-{Guid.NewGuid():N}";

        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Test sırasında TCP listener'ları başlatma.
                    services.RemoveAll<IHostedService>();

                    // Gerçek RabbitMQ, ayrı test kuyruğu.
                    services.PostConfigure<RabbitMQOptions>(options =>
                    {
                        options.Host = "localhost";
                        options.Port = 5672;
                        options.User = userName;
                        options.Password = password;
                        options.QueueName = queueName;
                        options.Durable = false;
                        options.Exclusive = false;
                        options.AutoDelete = false;
                        options.Arguments = null;
                    });
                });
            });

        await using (factory)
        using (var client = factory.CreateClient())
        {
            var response = await client.PostAsJsonAsync("/api/vehicle-data/34TEST123", new { Plate = "34TEST123" });

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        // Mesajı ayrı bir RabbitMQ bağlantısıyla oku.
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "fleetpulse",
            Password = password
        };

        await using var connection = await connectionFactory.CreateConnectionAsync();

        await using var channel = await connection.CreateChannelAsync();

        try
        {
            var result = await channel.BasicGetAsync(queueName, autoAck: true);

            Assert.NotNull(result);

            using var document = JsonDocument.Parse(result.Body.ToArray());

            Assert.Equal("34TEST123", document.RootElement.GetProperty("plate").GetString());

            Assert.True(document.RootElement.TryGetProperty("ReceivedAt", out _));
        }
        finally
        {
            await channel.QueueDeleteAsync(queueName);
        }
    }

    [Fact]
    public async Task PostVehicleData_WithEmptyPayload_ReturnsBadRequest()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IHostedService>();

                    services.PostConfigure<RabbitMQOptions>(options =>
                    {
                        options.User = "test";
                        options.Password = "test";
                    });
                });
            });

        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/vehicle-data/34TEST123", new { });

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
