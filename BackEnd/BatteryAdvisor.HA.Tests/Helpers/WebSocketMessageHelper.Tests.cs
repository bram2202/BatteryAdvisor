using System.Text.Json;
using BatteryAdvisor.HA.Helpers;

namespace BatteryAdvisor.HA.Tests.Helpers;

public class WebSocketMessageHelperTests
{
    [Fact]
    public void BuildAuthMessage_ReturnsValidJson()
    {
        var result = WebSocketMessageHelper.BuildAuthMessage("my-token");

        var doc = JsonDocument.Parse(result);
        Assert.Equal("auth", doc.RootElement.GetProperty("type").GetString());
        Assert.Equal("my-token", doc.RootElement.GetProperty("access_token").GetString());
    }

    [Fact]
    public void BuildListStatisticIdsMessage_ReturnsValidJson()
    {
        var result = WebSocketMessageHelper.BuildListStatisticIdsMessage(42);

        var doc = JsonDocument.Parse(result);
        Assert.Equal(42, doc.RootElement.GetProperty("id").GetInt32());
        Assert.Equal("recorder/list_statistic_ids", doc.RootElement.GetProperty("type").GetString());
        Assert.Equal("sum", doc.RootElement.GetProperty("statistic_type").GetString());
    }
}
