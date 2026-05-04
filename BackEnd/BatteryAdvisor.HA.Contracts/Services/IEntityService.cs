namespace BatteryAdvisor.HA.Contracts.Services;

public interface IHomeAssistantRestService
{
    /// <summary>
    /// Tests the connection to the Home Assistant API using the provided URL and token.
    /// </summary>
    /// <param name="url">The URL of the Home Assistant API.</param>
    /// <param name="token">The authentication token for the Home Assistant API.</param>
    /// <returns>Boolean indicating whether the connection was successful.</returns>
    Task<bool> TestConnectionAsync(string url, string token);
}