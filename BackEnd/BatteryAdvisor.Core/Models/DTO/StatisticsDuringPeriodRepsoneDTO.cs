using BatteryAdvisor.Core.Models.HomeAssistant;

namespace BatteryAdvisor.Core.Models.DTO;

public class StatisticsDuringPeriodResponseDTO
{

    public StatisticsDuringPeriodResponseDTO(StatisticsDuringPeriodModel model)
    {
        StartDateTimeUtc = ConvertUnixTimestampToUtcDateTime(model.Start);
        EndDateTimeUtc = ConvertUnixTimestampToUtcDateTime(model.End);
        Sum = model.Sum;
        Change = model.Change;
    }

    public DateTime StartDateTimeUtc { get; set; }

    public DateTime EndDateTimeUtc { get; set; }

    public double Sum { get; set; }

    public double Change { get; set; }


    private static DateTime ConvertUnixTimestampToUtcDateTime(long unixTimestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).UtcDateTime;
    }
}