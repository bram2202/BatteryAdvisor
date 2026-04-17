using System.Text.Json;

namespace BatteryAdvisor.Core.Models.Response;

public class WebSocketResponseModel
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Success { get; set; }
    public JsonElement Result { get; set; }
}