namespace Services;

using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

using Interfaces;
using Models;
using Helpers;

public class AstronomyPictureOfTheDayService : IAstronomyPictureOfTheDayService
{
    private readonly ApodConfiguration _config;
    private readonly ILogger<AstronomyPictureOfTheDayService> _logger;
    private readonly IKafkaProducer _producer;
    private readonly HttpClient _httpClient;
    private Dictionary<string, string> _dictionary;

    public AstronomyPictureOfTheDayService(HttpClient client, IOptions<ApodConfiguration> config, IKafkaProducer producer, ILogger<AstronomyPictureOfTheDayService> logger)
    {
        _config = config.Value;

        client.BaseAddress = new Uri(_config.BaseUrl);
        client.Timeout = new TimeSpan(0, 0, 30);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _httpClient = client;

        _logger = logger;

        _producer = producer;

        _dictionary = new Dictionary<string, string>();

    }

    /// <summary>
    /// Retrieves the Nasa Astronomy Picture of the Day
    /// <para>See the <see href="https://apod.nasa.gov/apod/astropix.html">website</see>.</para>
    /// </summary>
    /// <returns></returns>
    public async Task<AstronomyPictureOfTheDay> GetAstronomyPictureOfTheDayAsync()
    {
        var requestUri = string.Format(_config.ApodPath, _config.ApiKey);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = await _httpClient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var apod = stream.ReadAndDeserializeFromJson<AstronomyPictureOfTheDay>();

            // Send to Kafta
            await _producer.SendJsonAsync(_config.MessageTopic, apod, apod.Title, _dictionary);

            return apod;
        }
    }
}
