using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class RadiusServerDetails
{
    public RadiusServerDetails() { }
    
    [JsonPropertyName("IPAddress")]
    public string IPAddress { get; set; } = string.Empty;

    [JsonPropertyName("SecondaryIPAddress")]
    public string SecondaryIPAddress { get; set; } = string.Empty;

    [JsonPropertyName("ThirdIPAddress")]
    public string ThirdIPAddress { get; set; } = string.Empty;

    [JsonPropertyName("HealthURL")]
    public string HealthURL { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("Location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("ServerName")]
    public string ServerName { get; set; } = string.Empty;
}

