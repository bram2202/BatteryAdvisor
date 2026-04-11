using System.Text.Json;

namespace BatteryAdvisor.Core.Models.HomeAssistant;

public class WebSocketResponseModel
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Success { get; set; }

    // The "result" property can be of various types depending on the message type.
    // For simplicity, we can use JsonElement to represent it as a flexible JSON structure.
    public JsonElement Result { get; set; }


    // "result": [
    // 		{


}