using System.Text.Json.Serialization;

namespace BatteryAdvisor.Core.Models.HomeAssistant;

public class EntityModel
{
    [JsonPropertyName("entity_id")]
    public required string EntityId { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("attributes")]
    public AttributesModel? Attributes { get; set; }

    [JsonPropertyName("last_changed")]
    public string? LastChanged { get; set; }

    [JsonPropertyName("last_updated")]
    public string? LastUpdated { get; set; }
}