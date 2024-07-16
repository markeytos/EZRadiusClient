using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class APIResultModel
{
    [JsonPropertyName("Success")]
    public bool Success { get; set; }

    [JsonPropertyName("Message")]
    public string Message { get; set; }
}