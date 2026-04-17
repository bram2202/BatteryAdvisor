namespace BatteryAdvisor.HA.Contracts.Helpers;

public interface IWebSocketMessageHelper
{
    /// <summary>
    /// Builds the authentication message to be sent to Home Assistant for WebSocket authentication using the provided access token.
    /// </summary>
    /// <param name="accessToken">The access token to use for authentication.</param>
    /// <returns>The serialized authentication message.</returns>
    string BuildAuthMessage(string accessToken);

    /// <summary>    
    /// Builds the message to request the list of statistic IDs from Home Assistant via the WebSocket API, using the specified message ID.
    /// </summary>   
    /// <param name="id">The message ID to include in the request message.</param>
    /// <returns>The serialized message to request the list of statistic IDs.</returns>
    string BuildListStatisticIdsMessage(int id);

    /// <summary>
    /// Builds the message to request statistics during a specific period for a given statistic ID from Home Assistant
    /// </summary> 
    /// <param name="id">The message ID to include in the request message.</param>
    /// <param name="statisticId">The statistic ID for which to retrieve statistics.</param>
    /// <param name="startTime">The start time of the period for which to retrieve statistics.</param>
    /// <param name="endTime">The end time of the period for which to retrieve statistics.</param>
    /// <returns>The serialized message to request statistics during a specific period.</returns>
    string BuildGetStatisticDuringPeriodMessage(int id, string statisticId, string startTime, string endTime);

    /// <summary>
    /// Builds the message to request the list of entities from Home Assistant via the WebSocket API, using the specified message ID.
    /// </summary>
    /// <param name="id">The message ID to include in the request message.</param>
    /// <returns>The serialized message to request the list of entities.</returns>
    string BuildGetEntitiesMessage(int id);
}
