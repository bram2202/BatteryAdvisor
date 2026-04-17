using System.Text.Json.Serialization;

namespace BatteryAdvisor.Core.Models.HomeAssistant;

/// <summary>
/// This is as subset of all the attributes that Home Assistant can return. 
/// We only need these for our use case, so we don't need to model all of them.
/// </summary>
public class AttributesModel
{
    [JsonPropertyName("friendly_name")]
    public string? FriendlyName { get; set; }

    [JsonPropertyName("unit_of_measurement")]
    public string? UnitOfMeasurement { get; set; }

    [JsonPropertyName("state_class")]
    public string? StateClass { get; set; }

    [JsonPropertyName("device_class")]
    public string? DeviceClass { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

}