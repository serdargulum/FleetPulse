using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using VehicleDataSimulator;
using VehicleDataSimulator.Application.Interfaces;
using VehicleDataSimulator.Application.Services;
using VehicleDataSimulator.Application.Services.Generators;
using VehicleDataSimulator.Infrastructure.ExternalServices;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHealthChecks();

var gatewayBaseUrl = builder.Configuration["GatewaySettings:BaseUrl"]
    ?? throw new InvalidOperationException("GatewaySettings:BaseUrl yapılandırması bulunamadı!");

builder.Services.AddTransient<ITelemetryPayload, BmwVehiclePayload>();
builder.Services.AddTransient<ITelemetryPayload, ToyotaVehiclePayload>();
builder.Services.AddTransient<ITelemetryPayload, MazdaVehiclePayload>();
builder.Services.AddTransient<ITelemetryPayload, HondaVehiclePayload>();

builder.Services.AddHttpClient<IVehicleApiClient, VehicleApiClient>(client =>
{
    client.BaseAddress = new Uri(gatewayBaseUrl);
});

builder.Services.AddScoped<VehicleDataGeneratorService>();

builder.Services.AddHostedService<VehicleDataSimulator.Worker.VehicleDataWorker>();

builder.Services
    .AddHttpClient<IVehicleApiClient, VehicleApiClient>()
    .AddResilienceHandler("custom", pipeline =>
    {
        pipeline.AddCircuitBreaker(
            new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(response => (int)response.StatusCode >= 500),
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(60)
            });


        // Sıralama önemli: Timeout, Circuit Breaker'ın içinde çalışmalı
        // ki oluşan timeout hataları Circuit Breaker tarafından değerlendirilebilsin.
        pipeline.AddTimeout(TimeSpan.FromSeconds(10));
    });

var host = builder.Build();

host.MapHealthChecks("/health");

host.Run();