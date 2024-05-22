namespace Services;

using Microsoft.Extensions.Options;

using Ardalis.GuardClauses;
using Confluent.Kafka;

using Helpers;

public class KafkaBackgroundService : BackgroundService
{

    private readonly ILogger<KafkaBackgroundService> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public KafkaBackgroundService(IOptions<KafkaConfiguration> kafkaConfiguration, ILogger<KafkaBackgroundService> logger)
    {
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;

        _kafkaConfiguration.BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER") ?? _kafkaConfiguration.BootstrapServers;

        _logger.LogInformation($"KAFKA_SERVER -> '{Environment.GetEnvironmentVariable("KAFKA_SERVER")}'");
        _logger.LogInformation($"BootstrapServers -> '{_kafkaConfiguration.BootstrapServers}'");

    }

    // private IConsumer<string, string> CreateConsumer(KafkaConfiguration kafkaConfiguration)
    private IConsumer<string, string> CreateConsumer()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfiguration.BootstrapServers,
            GroupId = _kafkaConfiguration.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Debug = _kafkaConfiguration.Debug
        };

        var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

        _logger.LogInformation($"Subscribing -> '{_kafkaConfiguration.TopicName}'");

        consumer.Subscribe(_kafkaConfiguration.TopicName);

        _logger.LogInformation($"Subscribed to topic {_kafkaConfiguration.TopicName}");

        return consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = CreateConsumer();

        var task = Task.Run(() => ConsumeAsync(consumer, stoppingToken), stoppingToken);

        return task;
    }

    protected virtual async Task ConsumeAsync(IConsumer<string, string> consumer, CancellationToken stoppingToken)
    {
        _logger.LogInformation($"[ConsumeAsync] Name: {consumer.Name}");

        Guard.Against.Null(consumer, nameof(consumer));

        // This NOP is here to free the calling thread during startup.
        // If we don't do that the startup isn't finalised (log "Application started. Press Ctrl+C to shut down.") until a first message is received.
        await Task.Delay(1);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeMessage = consumer.Consume(stoppingToken);

                // here you can process your received message                   
                _logger.LogInformation($"receive message with key = '{consumeMessage.Message.Key}' Value = {consumeMessage.Message.Value}");

                // Finally commit the event (also this is why we disabled AutoCommit, to precisely wait after the event is processed to commit it)
                consumer.Commit(consumeMessage);
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();
            }
            catch (ConsumeException consumeException)
            {
                // it's possible to write mechanism to resubscribe   
                _logger.LogError(consumeException, "Unexpected Consume Exception raised");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occured.");
            }
        }
    }
}
