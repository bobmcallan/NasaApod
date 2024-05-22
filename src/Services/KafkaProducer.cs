namespace Services;

using System.Globalization;
using System.Text;

using Confluent.Kafka;
using Newtonsoft.Json;
using Ardalis.GuardClauses;

public class KafkaProducer : IKafkaProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private IProducer<string, string> _kafkaClient;

    public KafkaProducer(KafkaHandler client, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        _kafkaClient = new DependentProducerBuilder<string, string>(client.Handle)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
            .Build();
    }

    public async Task SendJsonAsync(string targetTopic, object message, string messageKey, Dictionary<string, string> headers)
    {
        Guard.Against.NullOrEmpty(targetTopic, nameof(targetTopic));
        Guard.Against.Null(message, nameof(message));

        try
        {
            var kafkaMessage = CreateMessage(message, headers, messageKey);

            var deliveryResult = await _kafkaClient.ProduceAsync(targetTopic, kafkaMessage);
            _logger.LogTrace(deliveryResult.Message.Value.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error producing for topic {targetTopic}: {e.Message}");
        }
    }

    private Message<string, string> CreateMessage(object value, Dictionary<string, string> headers, string messagekey)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            Culture = CultureInfo.InvariantCulture
        };
        string jsonPayload = JsonConvert.SerializeObject(value, serializerSettings);

        Message<string, string> kafkaMessage = new()
        {
            Value = jsonPayload
        };

        //Attaching a key to messages will ensure messages with the same key always go to the same partition in a topic.
        //Kafka guarantees order within a partition, but not across partitions in a topic, so alternatively not providing a key
        if (!Equals(messagekey, default(string)))
        {
            kafkaMessage.Key = messagekey;
        }

        if (headers != null)
        {
            kafkaMessage.Headers = new Headers();
            foreach (var item in headers)
            {
                if (string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrWhiteSpace(item.Value))
                {
                    continue;
                }
                kafkaMessage.Headers.Add(item.Key, Encoding.UTF8.GetBytes(item.Value));
            }
        }

        return kafkaMessage;
    }

    protected virtual void ErrorHandler(DeliveryReport<string, string> deliveryReport)
    {
        if (deliveryReport?.Status == PersistenceStatus.NotPersisted)
        {
            _logger.LogError($"Message: {deliveryReport.Message.Value} Error: {deliveryReport.Error} ");
        }
    }
}
