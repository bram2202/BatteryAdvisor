using BatteryAdvisor.HA.Clients;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly IApiClient _apiClient;

    public TestingController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
        await _apiClient.GetData();
        return Ok("Run completed");
    }
}
