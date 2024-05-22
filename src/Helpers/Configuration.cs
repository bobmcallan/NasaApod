namespace Helpers;

public class ApodConfiguration
{
    public string BaseUrl { get; set; }
    public string ApodPath { get; set; }
    public string ApiKey { get; set; }
    public string MessageTopic { get; set; }
}

public class KafkaConfiguration
{
    public string GroupId { get; set; }
    public string Debug { get; set; }
    public string TopicName { get; set; }
    public string BootstrapServers { get; set; }
}