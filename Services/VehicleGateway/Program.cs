using VehicleGateway.Controllers;
using VehicleGateway.Infrastructure.Messaging.RabbitMq;
using VehicleGateway.Infrastructure.DependencyInjection;
using VehicleGateway.Application.Interfaces;
using VehicleGateway.Application.Commands.StoreVehicleData;
using VehicleGateway.Infrastructure.Middleware;
using VehicleGateway.Infrastructure.TcpListener;
using System.Threading.RateLimiting;
using VehicleGateway.Options;
using Polly;
using Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddSingleton<IMessageProducer, RabbitMqMessageProducer>();
builder.Services.AddScoped<StoreVehicleDataHandler>();
builder.Services.AddHostedService<TeltonikaTcpListener>();
builder.Services.AddHostedService<QueclinkTcpListener>();

builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.AddOptions<RabbitMQOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMQ"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "RabbitMQ:HostName boş olamaz")
    .Validate(o => !string.IsNullOrWhiteSpace(o.User), "RabbitMQ:User boş olamaz")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "RabbitMQ:Password boş olamaz")
    .ValidateOnStart();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("VehiclePlatePolicy", httpContext =>
    {
        var plate = httpContext.Request.RouteValues["plate"]?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: plate,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,          // 1 dakikada izin verilen maksimum istek 
                Window = TimeSpan.FromMinutes(1), // Zaman penceresi
                QueueLimit = 2,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Çok sık veri gönderiliyor. Lütfen biraz bekleyin.", cancellationToken);
    };
});

builder.Services.AddResiliencePipeline("rabbitmq-publish", (pipelineBuilder, context) =>
{
    var logger = context.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RabbitMqPublishPipeline");

    pipelineBuilder.AddRetry(new RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        OnRetry = args =>
        {
            logger.LogWarning(
                args.Outcome.Exception,
                "RabbitMQ mesaj gönderimi başarısız oldu. {AttemptNumber}. deneme yapılıyor...",
                args.AttemptNumber);

            return ValueTask.CompletedTask;
        }
    });
});

// RabbitMQ healthcheck
var rabbit = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>()!;
var factory = new ConnectionFactory
{
    HostName = rabbit.Host,
    Port = rabbit.Port,
    UserName = rabbit.User,
    Password = rabbit.Password
};
builder.Services.AddHealthChecks().AddRabbitMQ(sp => factory.CreateConnectionAsync());

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/api/vehicle-data/{plate}", DataStoreController.Store)
   .RequireRateLimiting("VehiclePlatePolicy");

app.MapPost("/{any}", async (string any) =>
{
    Console.WriteLine(any);
});

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }