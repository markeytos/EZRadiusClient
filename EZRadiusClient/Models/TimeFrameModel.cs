using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace  EZRadiusClient.Models;

public class TimeFrameModel
{
    public TimeFrameModel() { }
    
    [JsonPropertyName("DateFrom")]
    public DateTime DateFrom { get; set; } = DateTime.Now.AddDays(-1);
    
    [JsonPropertyName("DateTo")]
    public DateTime DateTo { get; set; } = DateTime.Now;
}
