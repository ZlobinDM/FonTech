namespace FonTech.Producer;

public interface IMessageProducer
{
    void SendMessage<T>(T message, string routingKey, string? exchange = default);
}