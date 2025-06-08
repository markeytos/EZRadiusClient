using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;


public class AuditRequestModel
{
    public AuditRequestModel(TimeFrameModel timeFrame)
    {
        DateFrom = timeFrame.DateFrom;
        DateTo = timeFrame.DateTo;
    }
    
    public AuditRequestModel()
    {
        DateFrom = DateTime.UtcNow.AddDays(-60);
        DateTo = DateTime.UtcNow.AddDays(1);
    }

    [JsonPropertyName("DateFrom")]
    public DateTime? DateFrom { get; set; }

    [JsonPropertyName("DateTo")]
    public DateTime? DateTo { get; set; }

    [JsonPropertyName("MaxNumberOfRecords")]
    public int MaxNumberOfRecords { get; set; } = 5000;

    [JsonPropertyName("PageNumber")]
    public int PageNumber { get; set; } = 0;
}