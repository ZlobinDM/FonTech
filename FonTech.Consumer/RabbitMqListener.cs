using System.Diagnostics;
using System.Text;
using FonTech.Domain.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FonTech.Consumer;

public class RabbitMqListener : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _chanel;
    private readonly IOptions<RabbitMqSettings> _options;

    public RabbitMqListener(IOptions<RabbitMqSettings> options)
    {
        _options = options;
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _chanel = _connection.CreateModel();
        _chanel.QueueDeclare(_options.Value.QueueName, true, true, false, null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_chanel);
        consumer.Received += (obj, basicDeliver) =>
        {
            var content = Encoding.UTF8.GetString(basicDeliver.Body.ToArray());
            Debug.WriteLine("Полученно сообщение: {content}");
            
            _chanel.BasicAck(basicDeliver.DeliveryTag, false);
        };

        _chanel.BasicConsume(_options.Value.QueueName, false, consumer);
        
        Dispose();
        
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _chanel.Dispose();
        _connection.Dispose();
        base.Dispose();
    }
}