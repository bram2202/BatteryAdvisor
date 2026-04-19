using BatteryAdvisor.Core.Contracts.Enums;

namespace BatteryAdvisor.Core.Contracts.Models;

public class ConfigurationReadModel
{
    public required ConfigurationKeys Name { get; set; }

    public required string Value { get; set; }
}