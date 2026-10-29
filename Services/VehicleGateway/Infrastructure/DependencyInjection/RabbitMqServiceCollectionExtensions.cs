using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleGateway.Application.Interfaces;
using VehicleGateway.Infrastructure.Messaging.RabbitMq;

namespace VehicleGateway.Infrastructure.DependencyInjection;

public static class RabbitMqServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        // TCP bağlantısını yöneten sınıf Singleton olmalıdır
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        // Producer stateless (durumsuz) olduğu için Transient veya Singleton olarak kaydedilebilir
        services.AddTransient<IMessageProducer, RabbitMqMessageProducer>();

        return services;
    }
}