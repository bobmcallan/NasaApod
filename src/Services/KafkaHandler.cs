namespace Services;

using Microsoft.Extensions.Options;
using Confluent.Kafka;
using Ardalis.GuardClauses;

using Helpers;

public class KafkaHandler : IDisposable
{
    IProducer<byte[], byte[]> kafkaProducer;

    private readonly KafkaConfiguration _kafkaConfiguration;

    public KafkaHandler(IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        Guard.Against.Null(kafkaConfiguration, nameof(kafkaConfiguration));

        _kafkaConfiguration = kafkaConfiguration.Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER") ?? _kafkaConfiguration.BootstrapServers,
            Debug = kafkaConfiguration.Value.Debug
        };

        this.kafkaProducer = new ProducerBuilder<byte[], byte[]>(producerConfig).Build();
    }

    public Handle Handle { get => this.kafkaProducer.Handle; }

    public void Dispose()
    {
        kafkaProducer.Flush();
        kafkaProducer.Dispose();
    }
}
