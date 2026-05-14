using System.Text.Json.Serialization;
using BatteryAdvisor.Core.Contracts.Enums;

namespace BatteryAdvisor.Core.Contracts.Models;

public class ConfigurationReadModel
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ConfigurationKeys Name { get; set; }

    public required string Value { get; set; }
}