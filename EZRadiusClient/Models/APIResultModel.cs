using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class APIResultModel
{
    public APIResultModel() { }

    public APIResultModel(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    [JsonPropertyName("Success")]
    public bool Success { get; set; }

    [JsonPropertyName("Message")]
    public string? Message { get; set; }
}
