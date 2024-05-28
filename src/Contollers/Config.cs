namespace Controllers;

using Microsoft.AspNetCore.Mvc;

using Interfaces;
using Services;


[ApiController]
[Route("[controller]")]
public class ConfigController : ControllerBase
{
    private IConfigService _configService;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IConfigService configService, ILogger<ConfigController> logger)
    {
        _configService = configService;
        _logger = logger;

        _logger.LogInformation("Config Controler constructor complete");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var _output = await _configService.GetConfigAsync();
        return Ok(_output);
    }

}