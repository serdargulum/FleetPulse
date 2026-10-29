using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StreamWorker.Application.Interfaces;
using StreamWorker.Infrastructure.Messaging.RabbitMq;

namespace StreamWorker.Infrastructure.DependencyInjection;

public static class RabbitMqServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        services.AddTransient<IMessageConsumer, RabbitMqMessageConsumer>();

        return services;
    }
}