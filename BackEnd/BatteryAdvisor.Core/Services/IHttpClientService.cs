namespace BatteryAdvisor.Core.Services;

public interface IHttpClientService
{
    /// <summary>
    /// Gets the content of the specified URL and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>The deserialized response content.</returns>
    Task<T> GetAsync<T>(string url, IDictionary<string, string>? headers = null);

    /// <summary>
    /// Sends a POST request to the specified URL with the given data and headers, and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">The URL to send the POST request to.</param>
    /// <param name="data">The data to include in the request body.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>The deserialized response content.</returns>
    Task<T> PostAsync<T>(string url, object data, IDictionary<string, string>? headers = null);

    /// <summary>
    /// Sends a PUT request to the specified URL with the given data and headers, and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">The URL to send the PUT request to.</param>
    /// <param name="data">The data to include in the request body.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>The deserialized response content.</returns>
    Task<T> PutAsync<T>(string url, object data, IDictionary<string, string>? headers = null);

    /// <summary>
    /// Sends a DELETE request to the specified URL with the given headers.
    /// </summary>
    /// <param name="url">The URL to send the DELETE request to.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(string url, IDictionary<string, string>? headers = null);
}