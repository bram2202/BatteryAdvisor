using BatteryAdvisor.Core.Models.DTO;
using BatteryAdvisor.HA.Contracts.Clients;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatisticController : ControllerBase
{

    private readonly IWebSocketClient _homeAssistantWebSocketClient;


    public StatisticController(
        IWebSocketClient homeAssistantWebSocketClient
    )
    {
        _homeAssistantWebSocketClient = homeAssistantWebSocketClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatisticIds()
    {
        var result = await _homeAssistantWebSocketClient.GetStatisticIds();
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> GetStatisticsDuringPeriod([FromBody] StatisticsDuringPeriodRequestDTO request)
    {
        var result = await _homeAssistantWebSocketClient.GetStatisticsDuringPeriod(request.Id, request.Start, request.End);

        // Convert all the results to DTOs
        var dto = result.Select(r => new StatisticsDuringPeriodResponseDTO(r)).ToList();

        return Ok(dto);
    }

}