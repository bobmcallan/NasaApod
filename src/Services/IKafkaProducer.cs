namespace Services;
public interface IKafkaProducer
{
    Task SendJsonAsync(string targetTopic, object message, string messageKey, Dictionary<string, string> headers);
}
