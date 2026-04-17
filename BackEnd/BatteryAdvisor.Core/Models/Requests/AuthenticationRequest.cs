namespace BatteryAdvisor.Core.Models.Requests;

public class AuthenticationRequest
{
    public string? type { get; set; }
    public string? access_token { get; set; }
}

