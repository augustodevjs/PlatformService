﻿using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using PlatformService.Dtos;
using PlatformService.Contracts;

namespace PlatformService.Infraestructure.Messaging;

public class RabbitMQService : IMessageBusClient
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQService> _logger;

    public RabbitMQService(
        IConfiguration configuration,
        ILogger<RabbitMQService> logger
    )
    {
        _logger = logger;
        _configuration = configuration;
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]!)
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _channel.QueueDeclare(queue: "platformQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: "platformQueue", exchange: "trigger", routingKey: "");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            _logger.LogInformation("--> Connect to MessageBus");
        }
        catch (Exception ex)
        {
            _logger.LogError($"--> An unexpected error occurred: {ex.Message}");
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublished)
    {
        var message = JsonSerializer.Serialize(platformPublished);

        if (_connection.IsOpen)
        {
            _logger.LogInformation("--> RabbitMQ Connection Open, sending message...");

            SendMessage(message);
        }
        else
        {
            _logger.LogInformation("--> RabbitMQ Connection closed, not sending message...");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body);

        _logger.LogInformation($"--> We have sent {message}");
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("--> RabbitMQ Connection Shutdown");
    }

    public void Dispose()
    {
        _logger.LogInformation($"MessageBus Disposed");

        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
