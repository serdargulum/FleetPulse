using StreamWorker.Application.Interfaces;
using StreamWorker.Application.Services;
using StreamWorker.Application.Services.VehicleDataParser;
using StreamWorker.Application.Services.VehicleDataParser.Parsers;
using StreamWorker.Infrastructure.DependencyInjection;
using StreamWorker.Infrastructure.Messaging.RabbitMq;
using StreamWorker.Infrastructure.Persistence;
using StreamWorker.Infrastructure.Realtime;
using StreamWorker.Options;
using StreamWorker.Worker;
using Polly;
using Polly.Registry;
using Polly.Retry;
using Npgsql;
using StreamWorker.Application.Services.VehicleDataParser.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using HealthChecks.NpgSql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection connection string is missing.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddResiliencePipeline("postgres-retry", pipeline =>
{
    pipeline.AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        ShouldHandle = new PredicateBuilder().Handle<NpgsqlException>(ex => ex.IsTransient)
    });
});

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddSingleton<IMessageConsumer, RabbitMqMessageConsumer>();

builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

builder.Services.AddSingleton<IVehicleDataParser, BmwVehiclePayload>();
builder.Services.AddSingleton<IVehicleDataParser, MazdaVehiclePayload>();
builder.Services.AddSingleton<IVehicleDataParser, ToyotaVehiclePayload>();
builder.Services.AddSingleton<IVehicleDataParser, HondaVehiclePayload>();
builder.Services.AddScoped<IVehicleTelemetryProcessor, VehicleTelemetryProcessor>();

builder.Services.AddTransient<IVehicleDataParserValidator, ToyotaVehiclePayloadValidator>();
builder.Services.AddTransient<IVehicleDataParserValidator, MazdaVehiclePayloadValidator>();
builder.Services.AddTransient<IVehicleDataParserValidator, BmwVehiclePayloadValidator>();
builder.Services.AddTransient<IVehicleDataParserValidator, HondaVehiclePayloadValidator>();

builder.Services.AddSingleton<VehicleDataParser>();
builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.AddHostedService<VehicleQueueConsumerWorker>();

builder.Services.AddSignalR();

builder.Services.AddOptions<RabbitMQOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMQ"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "RabbitMQ:Password boş olamaz")
    .ValidateOnStart();

// healthcheck - RabbitMQ - PostGresql 
var rabbit = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>()!;
var factory = new ConnectionFactory
{
    HostName = rabbit.Host,
    Port = rabbit.Port,
    UserName = rabbit.User,
    Password = rabbit.Password
};
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddRabbitMQ(sp => factory.CreateConnectionAsync());

var app = builder.Build();

app.UseCors("Frontend");

app.MapHub<TelemetryHub>("/hubs/telemetry");

app.MapHealthChecks("/health");

app.Run();