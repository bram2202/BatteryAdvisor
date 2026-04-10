namespace BatteryAdvisor.Core.ApplicationOptions;

public class ApplicationOptions
{
    public HomeAssistantOptions HomeAssistant { get; set; } = new();
}

public class HomeAssistantOptions
{
    public string Url { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}