using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace EZRadiusClient.Models;

public class AllowedIPAddressModel
{
    public AllowedIPAddressModel(string clientIpAddress, string sharedSecret)
    {
        ClientIPAddress = clientIpAddress;
        SharedSecret = sharedSecret;
    }

    [JsonPropertyName("ClientIPAddress")]
    [Name("ClientIPAddress")]
    public string ClientIPAddress { get; set; }

    [JsonPropertyName("SharedSecret")]
    [Name("SharedSecret")]
    public string SharedSecret { get; set; }
}