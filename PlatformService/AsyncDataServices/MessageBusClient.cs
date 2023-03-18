using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection = default!;
    private readonly IModel _channel = default!;

    public MessageBusClient(IConfiguration configuration)
    {
        ConnectionFactory factory;

        _configuration = configuration;
        factory = new()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"] ?? default!)
        };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("trigger", ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

            Console.WriteLine("--> Connected to Message Bus.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");
        }
    }

    public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
    {
        var message = JsonSerializer.Serialize(platformPublishDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ: Connection opened, sending message...");

            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ: Connection closed, not sending.");
        }
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");

        if (_channel.IsClosed) return;
        _channel.Close();
        _connection.Close();
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish("trigger", "", false, null, body);
        Console.WriteLine($"--> RabbitMQ: Sent message - {message}");
    }

    private void RabbitMQ_ConnectionShutDown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ: Connection shut down.");
    }
}