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

        var message = new VendaStatusMessage(vendaId, expectedStatus);
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish(exchange: string.Empty, routingKey: queueName, mandatory: false, basicProperties: properties, body: body);

        return Task.CompletedTask;
    }
}
