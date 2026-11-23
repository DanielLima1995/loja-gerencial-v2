using System.Text.Json;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Loja.API.Messaging;

public sealed class VendasStatusConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IVendasStatusQueue _vendasStatusQueue;
    private readonly ILogger<VendasStatusConsumer> _logger;

    public VendasStatusConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        IVendasStatusQueue vendasStatusQueue,
        ILogger<VendasStatusConsumer> logger)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
        _vendasStatusQueue = vendasStatusQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var rabbitSettings = _configuration.GetSection("RabbitMQ");
        var queueName = rabbitSettings["Queue"] ?? throw new InvalidOperationException("RabbitMQ:Queue não foi configurada.");
        var delaySeconds = rabbitSettings.GetValue<int>("DelaySeconds", 60);
        if (delaySeconds <= 0)
        {
            throw new InvalidOperationException("RabbitMQ:DelaySeconds deve ser maior que zero.");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = rabbitSettings["Host"] ?? "localhost",
                    Port = rabbitSettings.GetValue<int>("Port", 5672),
                    UserName = rabbitSettings["Username"] ?? throw new InvalidOperationException("RabbitMQ:Username não foi configurado."),
                    Password = rabbitSettings["Password"] ?? throw new InvalidOperationException("RabbitMQ:Password não foi configurada."),
                    DispatchConsumersAsync = true
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var connectionClosed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                connection.ConnectionShutdown += (_, _) => connectionClosed.TrySetResult(true);
                channel.ModelShutdown += (_, _) => connectionClosed.TrySetResult(true);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (_, eventArgs) =>
                {
                    VendaStatusMessage? message;
                    try
                    {
                        message = JsonSerializer.Deserialize<VendaStatusMessage>(eventArgs.Body.Span);
                        if (message is null || message.VendaId <= 0 || string.IsNullOrWhiteSpace(message.ExpectedStatus))
                        {
                            _logger.LogWarning("Mensagem inválida recebida da fila {Queue}.", queueName);
                            channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
                            return;
                        }
                    }
                    catch (JsonException exception)
                    {
                        _logger.LogError(exception, "Mensagem JSON inválida recebida da fila {Queue}.", queueName);
                        channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
                        return;
                    }

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var vendasService = scope.ServiceProvider.GetRequiredService<IVendasService>();
                        var venda = await vendasService.AdvanceStatus(message.VendaId, message.ExpectedStatus);

                        if (venda is not null)
                        {
                            _logger.LogInformation("Status da venda {VendaId} alterado de '{StatusAnterior}' para '{NovoStatus}' e registrado no histórico.", venda.Id, message.ExpectedStatus, venda.Status);

                            var statusIndex = Vendas.StatusEtapas.ToList().IndexOf(venda.Status);
                            if (statusIndex >= 0 && statusIndex < Vendas.StatusEtapas.Count - 1)
                            {
                                await _vendasStatusQueue.PublishAsync(venda.Id, venda.Status, stoppingToken);
                                _logger.LogInformation("Próxima etapa da venda {VendaId} agendada para {DelaySeconds} segundos.", venda.Id, delaySeconds);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Mensagem da venda {VendaId} ignorada: a venda não existe ou o status atual não corresponde a '{StatusEsperado}'.", message.VendaId, message.ExpectedStatus);
                        }

                        channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        if (channel.IsOpen)
                        {
                            channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Falha ao avançar o status da venda {VendaId}; a mensagem será reprocessada.", message.VendaId);
                        if (channel.IsOpen)
                        {
                            channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                };

                channel.BasicConsume(queue: queueName, autoAck: false, exclusive: true, consumer: consumer);
                _logger.LogInformation("Consumidor RabbitMQ exclusivo ativo para a fila {Queue}.", queueName);

                await Task.WhenAny(connectionClosed.Task, Task.Delay(Timeout.Infinite, stoppingToken));
                if (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Conexão com RabbitMQ encerrada; uma nova tentativa será realizada.");
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Não foi possível conectar ou registrar o consumidor exclusivo no RabbitMQ; nova tentativa em 5 segundos.");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
