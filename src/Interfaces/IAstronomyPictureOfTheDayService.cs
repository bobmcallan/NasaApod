namespace Interfaces;

using Models;

public interface IAstronomyPictureOfTheDayService
{
    /// <summary>
    /// Retrieves the Nasa Astronomy Picture of the Day
    /// </summary>
    /// <returns></returns>
    Task<AstronomyPictureOfTheDay> GetAstronomyPictureOfTheDayAsync();
}
