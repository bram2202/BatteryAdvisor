using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeAssistantController : ControllerBase
{
    private readonly IHomeAssistantRestService _homeAssistantService;

    public HomeAssistantController(IHomeAssistantRestService homeAssistantService)
    {
        _homeAssistantService = homeAssistantService;
    }

    [HttpGet("test-connection")]
    public async Task<IActionResult> TestConnection([FromQuery] string url, [FromQuery] string token)
    {
        try
        {
            var result = await _homeAssistantService.TestConnectionAsync(url, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}