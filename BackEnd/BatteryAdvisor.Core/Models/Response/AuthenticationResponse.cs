namespace BatteryAdvisor.Core.Models.Response;

public class AuthenticationResponse
{
    public string? type { get; set; }
    public string? ha_version { get; set; }
    public string? message { get; set; }
}