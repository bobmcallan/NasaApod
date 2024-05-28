namespace Controllers;

using Microsoft.AspNetCore.Mvc;

using Interfaces;
using Services;

[ApiController]
[Route("[controller]")]
public class ApodController : ControllerBase
{
    private IAstronomyPictureOfTheDayService _apodService;

    public ApodController(IAstronomyPictureOfTheDayService apodService)
    {
        _apodService = apodService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var _output = await _apodService.GetAstronomyPictureOfTheDayAsync();
        return Ok(_output);
    }

}