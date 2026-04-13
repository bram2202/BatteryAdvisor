using System.Text.Json;

namespace BatteryAdvisor.Core.Models.HomeAssistant;

public class WebSocketResponseModel
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Success { get; set; }
    public JsonElement Result { get; set; }
}