
namespace StreamWorker.Options;

public class RabbitMQOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string QueueName { get; set; } = "";

    public bool Durable { get; set; }
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }

    public IDictionary<string, object?>? Arguments { get; set; }
}
