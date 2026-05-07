using BatteryAdvisor.Core.Models.DTO;
using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatteryAdvisor.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EntityController : ControllerBase
{
    private readonly IEntityService _entityService;

    public EntityController(IEntityService entityService)
    {
        _entityService = entityService;
    }

    [HttpGet("entities")]
    public async Task<IActionResult> GetStatisticEntities()
    {
        var statisticEntities = await _entityService.GetStatisticEntities();
        return Ok(statisticEntities);
    }

    [HttpPost("statistic-entities")]
    public async Task<IActionResult> SaveStatisticEntity([FromBody] StatisticEntitiesSaveDto statisticEntity)
    {
        try
        {
            await _entityService.SaveStatisticEntities(statisticEntity);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}