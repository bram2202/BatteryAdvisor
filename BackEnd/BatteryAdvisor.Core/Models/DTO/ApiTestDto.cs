using System.Text.Json.Serialization;

namespace BatteryAdvisor.Core.Models.DTO;

public class ApiTestDto
{    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}