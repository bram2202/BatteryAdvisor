using BatteryAdvisor.HA.Clients;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly RestClient _homeAssistantRestClient;

    public TestingController(RestClient homeAssistantRestClient)
    {
        _homeAssistantRestClient = homeAssistantRestClient;
    }


    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
        await _homeAssistantRestClient.GetData();
        return Ok("Run completed");
    }
}
