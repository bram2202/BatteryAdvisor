using System.Text.Json;
using BatteryAdvisor.HA.Helpers;

namespace BatteryAdvisor.HA.Tests.Helpers;

public class WebSocketMessageHelperTests
{
    private readonly WebSocketMessageHelper _helper = new();

    [Fact]
    public void BuildAuthMessage_ReturnsValidJson()
    {
        var result = _helper.BuildAuthMessage("my-token");

        var doc = JsonDocument.Parse(result);
        Assert.Equal("auth", doc.RootElement.GetProperty("type").GetString());
        Assert.Equal("my-token", doc.RootElement.GetProperty("access_token").GetString());
    }

    [Fact]
    public void BuildListStatisticIdsMessage_ReturnsValidJson()
    {
        var result = _helper.BuildListStatisticIdsMessage(42);

        var doc = JsonDocument.Parse(result);
        Assert.Equal(42, doc.RootElement.GetProperty("id").GetInt32());
        Assert.Equal("recorder/list_statistic_ids", doc.RootElement.GetProperty("type").GetString());
        Assert.Equal("sum", doc.RootElement.GetProperty("statistic_type").GetString());
    }

    [Fact]
    public void BuildGetStatisticDuringPeriodMessage_ReturnsValidJson()
    {
        var result = _helper.BuildGetStatisticDuringPeriodMessage(
            7,
            "sensor.battery_total",
            "2025-01-01T00:00:00Z",
            "2025-02-01T00:00:00Z");

        var doc = JsonDocument.Parse(result);
        var root = doc.RootElement;

        Assert.Equal(7, root.GetProperty("id").GetInt32());
        Assert.Equal("recorder/statistics_during_period", root.GetProperty("type").GetString());
        Assert.Equal("sensor.battery_total", root.GetProperty("statistic_ids")[0].GetString());
        Assert.Equal("2025-01-01T00:00:00Z", root.GetProperty("start_time").GetString());
        Assert.Equal("2025-02-01T00:00:00Z", root.GetProperty("end_time").GetString());
        Assert.Equal("month", root.GetProperty("period").GetString());
        Assert.Equal("sum", root.GetProperty("types")[0].GetString());
        Assert.Equal("change", root.GetProperty("types")[1].GetString());
    }

    [Fact]
    public void BuildGetEntitiesMessage_ReturnsValidJson()
    {
        var result = _helper.BuildGetEntitiesMessage(55);

        var doc = JsonDocument.Parse(result);
        Assert.Equal(55, doc.RootElement.GetProperty("id").GetInt32());
        Assert.Equal("get_states", doc.RootElement.GetProperty("type").GetString());
    }
}
