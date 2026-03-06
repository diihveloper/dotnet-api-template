#if RABBITMQ
namespace DiihTemplate.Infra.Messaging;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";

    public required string HostName { get; set; }
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string? DefaultExchange { get; set; }
}
#endif
