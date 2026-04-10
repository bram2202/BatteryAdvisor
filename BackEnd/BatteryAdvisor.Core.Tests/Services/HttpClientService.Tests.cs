using System.Net;
using BatteryAdvisor.Core.Services;

namespace BatteryAdvisor.Core.Tests;

public class HttpClientServiceTests
{
    [Fact]
    public async Task GetAsync_AddsAuthorizationHeader()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok")
            };
        });

        var httpClient = new HttpClient(handler);
        var service = new HttpClientService(httpClient);

        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = "Bearer test-token"
        };

        var result = await service.GetAsync<string>("https://example.com/status", headers);

        Assert.Equal("ok", result);
        Assert.NotNull(capturedRequest);
        Assert.True(capturedRequest!.Headers.TryGetValues("Authorization", out var values));
        Assert.Equal("Bearer test-token", Assert.Single(values));
    }

    [Fact]
    public async Task PostAsync_AppliesContentTypeHeaderToContent()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("created")
            };
        });

        var httpClient = new HttpClient(handler);
        var service = new HttpClientService(httpClient);

        var headers = new Dictionary<string, string>
        {
            ["Content-Type"] = "application/problem+json"
        };

        var result = await service.PostAsync<string>("https://example.com/items", new { Id = 1 }, headers);

        Assert.Equal("created", result);
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest!.Content);
        Assert.Equal("application/problem+json", capturedRequest.Content!.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task DeleteAsync_AddsCustomHeader()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        var httpClient = new HttpClient(handler);
        var service = new HttpClientService(httpClient);

        var headers = new Dictionary<string, string>
        {
            ["X-Correlation-Id"] = "abc-123"
        };

        await service.DeleteAsync("https://example.com/items/1", headers);

        Assert.NotNull(capturedRequest);
        Assert.True(capturedRequest!.Headers.TryGetValues("X-Correlation-Id", out var values));
        Assert.Equal("abc-123", Assert.Single(values));
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }
}
