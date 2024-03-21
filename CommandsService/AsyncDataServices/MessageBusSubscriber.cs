using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CommandsService.Contracts;

namespace CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private IModel _channel;
    private IConnection _connection;
    private IEventProcessor _eventProcessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MessageBusSubscriber> _logger;

    public MessageBusSubscriber(
        IConfiguration configuration, 
        IEventProcessor eventProcessor,
        ILogger<MessageBusSubscriber> logger
    )
    {
        _logger = logger;
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]!),
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "platformQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        _logger.LogInformation("--> Listenting on the Message Bus...");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShitdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (ModuleHandle, eventArgs) =>
        {
            _logger.LogInformation("--> Event Received!");

            var body = eventArgs.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        _channel.BasicConsume(queue: "platformQueue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private void RabbitMQ_ConnectionShitdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("--> Connection Shutdown");
    }

    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
    }
}
