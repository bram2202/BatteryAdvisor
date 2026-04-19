using BatteryAdvisor.Core.Contracts.Enums;

namespace BatteryAdvisor.Core.Models.Database;

public class ConfigurationModel : AbstractEntity
{
    public required ConfigurationKeys Name { get; set; }

    public required string Value { get; set; }
}

