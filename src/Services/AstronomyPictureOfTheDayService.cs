namespace Services;

using System.Net.Http.Headers;

using Helpers;
using Models;

public class AstronomyPictureOfTheDayService : IAstronomyPictureOfTheDayService
{
    private readonly HttpClient _httpClient;

    public AstronomyPictureOfTheDayService(HttpClient client)
    {
        client.BaseAddress = new Uri("https://api.nasa.gov/");
        client.Timeout = new TimeSpan(0, 0, 30);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient = client;
    }

    /// <summary>
    /// Retrieves the Nasa Astronomy Picture of the Day
    /// <para>See the <see href="https://apod.nasa.gov/apod/astropix.html">website</see>.</para>
    /// </summary>
    /// <returns></returns>
    public async Task<AstronomyPictureOfTheDay> GetAstronomyPictureOfTheDayAsync()
    {
        var requestUri = string.Format("https://api.nasa.gov/planetary/apod?api_key={0}", Constants.NASA_API_KEY);

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = await _httpClient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var astronomyPictureOfTheDay = stream.ReadAndDeserializeFromJson<AstronomyPictureOfTheDay>();
            return astronomyPictureOfTheDay;
        }
    }
}
