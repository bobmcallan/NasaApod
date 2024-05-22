namespace Services;

using Microsoft.Extensions.Options;
using Confluent.Kafka;
using Ardalis.GuardClauses;

using Helpers;

public class KafkaClientHandle : IDisposable
{
    IProducer<byte[], byte[]> kafkaProducer;

    public KafkaClientHandle(IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        Guard.Against.Null(kafkaConfiguration, nameof(kafkaConfiguration));

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaConfiguration.Value.BootstrapServers,
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
