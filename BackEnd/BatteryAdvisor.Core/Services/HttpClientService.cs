using System.Text.Json;

namespace BatteryAdvisor.Core.Services;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Gets the content of the specified URL and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>The deserialized response content.</returns>
    public async Task<T> GetAsync<T>(string url, IDictionary<string, string>? headers = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        AddHeaders(request, headers);

        using var response = await _httpClient.SendAsync(request);
        return await HandleResponseAsync<T>(response);
    }

    /// <summary>
    /// Sends a POST request to the specified URL with the given data and headers, and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">The URL to send the POST request to.</param>
    /// <param name="data">The data to include in the request body.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>The deserialized response content.</returns>
    public async Task<T> PostAsync<T>(string url, object data, IDictionary<string, string>? headers = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = CreateJsonContent(data)
        };

        AddHeaders(request, headers);

        using var response = await _httpClient.SendAsync(request);
        return await HandleResponseAsync<T>(response);
    }

    /// <summary>
    /// Sends a PUT request to the specified URL with the given data and headers, and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">The URL to send the PUT request to.</param>
    /// <param name="data">The data to include in the request body.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>The deserialized response content.</returns>
    public async Task<T> PutAsync<T>(string url, object data, IDictionary<string, string>? headers = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = CreateJsonContent(data)
        };

        AddHeaders(request, headers);

        using var response = await _httpClient.SendAsync(request);
        return await HandleResponseAsync<T>(response);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URL with the given headers.
    /// </summary>
    /// <param name="url">The URL to send the DELETE request to.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteAsync(string url, IDictionary<string, string>? headers = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        AddHeaders(request, headers);

        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessStatusCodeAsync(response);
    }


    /// <summary>
    /// Creates a StringContent object with the given data serialized as JSON.
    /// </summary>
    /// <param name="data">The data to serialize and include in the request body.</param>
    /// <returns>A StringContent object containing the serialized JSON data.</returns>
    private static StringContent CreateJsonContent(object data)
    {
        var json = JsonSerializer.Serialize(data);
        return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    }


    /// <summary>
    /// Adds the specified headers to the given HttpRequestMessage. If a "Content-Type" header is included and the request has content, it sets the content type accordingly.
    /// </summary>
    /// <param name="request">The HttpRequestMessage to add headers to.</param>
    /// <param name="headers">The headers to add to the request.</param>
    private static void AddHeaders(HttpRequestMessage request, IDictionary<string, string>? headers)
    {
        if (headers is null)
        {
            return;
        }

        foreach (var (key, value) in headers)
        {
            if (string.Equals(key, "Content-Type", StringComparison.OrdinalIgnoreCase) && request.Content is not null)
            {
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(value);
                continue;
            }

            if (!request.Headers.TryAddWithoutValidation(key, value) && request.Content is not null)
            {
                request.Content.Headers.TryAddWithoutValidation(key, value);
            }
        }
    }

    /// <summary>
    /// Handles the response from an HTTP request and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="response">The HttpResponseMessage to handle.</param>
    /// <returns>The deserialized response content.</returns>
    /// <exception cref="HttpRequestException">Thrown when the response status code indicates an error.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response body cannot be deserialized to the requested type.</exception>
    private static async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        await EnsureSuccessStatusCodeAsync(response);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (typeof(T) == typeof(string))
        {
            return (T)(object)responseBody;
        }

        var result = JsonSerializer.Deserialize<T>(responseBody, SerializerOptions);
        return result ?? throw new InvalidOperationException("The response body could not be deserialized to the requested type.");
    }

    /// <summary>
    /// Ensures that the response status code indicates success.
    /// </summary>
    /// <param name="response">The HttpResponseMessage to check.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the response status code indicates an error.</exception>
    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}). Response body: {errorBody}",
            null,
            response.StatusCode);
    }
}