using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class TimeFrameModel
{
    public TimeFrameModel(int days)
    {
        DateFrom = DateTime.Now.AddDays(-days);
        DateTo = DateTime.Now;
    }

    public TimeFrameModel(DateTime dateFrom, DateTime dateTo)
    {
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    [JsonPropertyName("DateFrom")]
    public DateTime DateFrom { get; set; }

    [JsonPropertyName("DateTo")]
    public DateTime DateTo { get; set; }
}
