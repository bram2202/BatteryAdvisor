using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EntityController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public EntityController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("entities")]
    public async Task<IActionResult> GetStatisticEntities()
    {
        var statisticEntities = await _statisticsService.GetStatisticEntities();
        return Ok(statisticEntities);
    }
}