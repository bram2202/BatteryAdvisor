using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.HA.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace BatteryAdvisor.HA.Tests.Services;

public class HomeAssistantRestServiceTests
{
    [Fact]
    public async Task TestConnectionAsync_ReturnsTrue_WhenRequestSucceeds()
    {
        // Arrange
        var httpClientService = new FakeHttpClientService();
        var service = new HomeAssistantRestService(httpClientService, NullLogger<HomeAssistantRestService>.Instance);

        // Act
        var result = await service.TestConnectionAsync("https://ha.local", "abc123");

        // Assert
        Assert.True(result);
        Assert.Equal("https://ha.local/api/", httpClientService.LastUrl);
        Assert.NotNull(httpClientService.LastHeaders);
        Assert.Equal("Bearer abc123", httpClientService.LastHeaders!["Authorization"]);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task TestConnectionAsync_Rethrows_ForBadRequestAndUnauthorized(HttpStatusCode statusCode)
    {
        // Arrange
        var exception = new HttpRequestException("Request failed", null, statusCode);
        var httpClientService = new FakeHttpClientService
        {
            ExceptionToThrow = exception
        };
        var service = new HomeAssistantRestService(httpClientService, NullLogger<HomeAssistantRestService>.Instance);

        // Act
        var action = () => service.TestConnectionAsync("https://ha.local", "abc123");

        // Assert
        var thrown = await Assert.ThrowsAsync<HttpRequestException>(action);
        Assert.Equal(statusCode, thrown.StatusCode);
    }

    [Fact]
    public async Task TestConnectionAsync_Rethrows_ForOtherHttpStatusCodes()
    {
        // Arrange
        var exception = new HttpRequestException("Server error", null, HttpStatusCode.InternalServerError);
        var httpClientService = new FakeHttpClientService
        {
            ExceptionToThrow = exception
        };
        var service = new HomeAssistantRestService(httpClientService, NullLogger<HomeAssistantRestService>.Instance);

        // Act
        var action = () => service.TestConnectionAsync("https://ha.local", "abc123");

        // Assert
        var thrown = await Assert.ThrowsAsync<HttpRequestException>(action);
        Assert.Equal(HttpStatusCode.InternalServerError, thrown.StatusCode);
    }

    private sealed class FakeHttpClientService : IHttpClientService
    {
        public Exception? ExceptionToThrow { get; set; }
        public string? LastUrl { get; private set; }
        public IDictionary<string, string>? LastHeaders { get; private set; }

        public Task<T> GetAsync<T>(string url, IDictionary<string, string>? headers = null)
        {
            LastUrl = url;
            LastHeaders = headers;

            if (ExceptionToThrow is not null)
            {
                return Task.FromException<T>(ExceptionToThrow);
            }

            return Task.FromResult((T)(object)new object());
        }

        public Task<T> PostAsync<T>(string url, object data, IDictionary<string, string>? headers = null)
            => throw new NotSupportedException();

        public Task<T> PutAsync<T>(string url, object data, IDictionary<string, string>? headers = null)
            => throw new NotSupportedException();

        public Task DeleteAsync(string url, IDictionary<string, string>? headers = null)
            => throw new NotSupportedException();
    }
}
