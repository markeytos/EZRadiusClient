using System.Text.Json.Serialization;

namespace  EZRadiusClient.Models;

public class TimeFrameModel
{
    [JsonPropertyName("DateFrom")]
    public DateTime DateFrom { get; set; } = DateTime.Now.AddDays(-2);
    
    [JsonPropertyName("DateTo")]
    public DateTime DateTo { get; set; } = DateTime.Now;
}
