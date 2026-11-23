using System.Text.Json;
using RabbitMQ.Client;

namespace Loja.API.Messaging;

public sealed class RabbitVendasStatusQueue : IVendasStatusQueue
{
    private readonly IConfiguration _configuration;

    public RabbitVendasStatusQueue(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task PublishAsync(int vendaId, string expectedStatus, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rabbitSettings = _configuration.GetSection("RabbitMQ");
        var queueName = rabbitSettings["Queue"] ?? throw new InvalidOperationException("RabbitMQ:Queue não foi configurada.");
        var delayQueueName = $"{queueName}-delay";
        var delaySeconds = rabbitSettings.GetValue<int>("DelaySeconds", 60);
        if (delaySeconds <= 0)
        {
            throw new InvalidOperationException("RabbitMQ:DelaySeconds deve ser maior que zero.");
        }

        var delayMilliseconds = checked(delaySeconds * 1000);
        var factory = new ConnectionFactory
        {
            HostName = rabbitSettings["Host"] ?? "localhost",
            Port = rabbitSettings.GetValue<int>("Port", 5672),
            UserName = rabbitSettings["Username"] ?? throw new InvalidOperationException("RabbitMQ:Username não foi configurado."),
            Password = rabbitSettings["Password"] ?? throw new InvalidOperationException("RabbitMQ:Password não foi configurada.")
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare(
            queue: delayQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-message-ttl"] = delayMilliseconds,
                ["x-dead-letter-exchange"] = string.Empty,
                ["x-dead-letter-routing-key"] = queueName
            });

        var message = new VendaStatusMessage(vendaId, expectedStatus);
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish(exchange: string.Empty, routingKey: delayQueueName, mandatory: false, basicProperties: properties, body: body);

        return Task.CompletedTask;
    }
}
