using BatteryAdvisor.HA.Clients;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly IRestClient _homeAssistantRestClient;
    private readonly IWebSocketClient _homeAssistantWebSocketClient;

    public TestingController(IRestClient homeAssistantRestClient, IWebSocketClient homeAssistantWebSocketClient)
    {
        _homeAssistantRestClient = homeAssistantRestClient;
        _homeAssistantWebSocketClient = homeAssistantWebSocketClient;
    }


    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
        await _homeAssistantRestClient.GetData();
        return Ok("Run completed");
    }

    [HttpGet("ws")]
    public async Task<IActionResult> RunWebSocket()
    {
        var result = await _homeAssistantWebSocketClient.GetStatisticIds();
        return Ok(result);
    }
}
