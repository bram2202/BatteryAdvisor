using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetConfiguration()
    {
        var result = await _configurationService.GetAllConfigurationsAsync();
        return Ok(result);
    }

    [HttpGet("by-key")]
    public async Task<IActionResult> GetConfigurationByKey([FromQuery] string key)
    {
        if (!Enum.TryParse<ConfigurationKeys>(key, true, out var configurationKey) ||
            !Enum.IsDefined(configurationKey))
        {
            return BadRequest($"Invalid configuration key: {key}");
        }

        var result = await _configurationService.GetConfigurationAsync(configurationKey);
        if (result is null)
        {
            return NotFound($"Configuration with key '{key}' not found.");
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddConfiguration([FromBody] ConfigurationCreateModel configuration)
    {
        try
        {
            await _configurationService.AddAsync(configuration);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateConfiguration([FromBody] ConfigurationCreateModel configuration)
    {
        try
        {
            await _configurationService.UpdateConfigurationAsync(configuration);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteConfiguration([FromQuery] string key)
    {
        try
        {
            if (!Enum.TryParse<ConfigurationKeys>(key, true, out var configurationKey) ||
                !Enum.IsDefined(configurationKey))
            {
                return BadRequest($"Invalid configuration key: {key}");
            }

            await _configurationService.DeleteConfigurationAsync(configurationKey);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
