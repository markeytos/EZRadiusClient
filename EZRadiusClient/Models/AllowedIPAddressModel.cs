using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace EZRadiusClient.Models;

public class AllowedIPAddressModel
{
    public AllowedIPAddressModel()
    {
        ClientIPAddress = String.Empty;
        SharedSecret = String.Empty;
        FriendlyName = String.Empty;
    }

    public AllowedIPAddressModel(string clientIpAddress, string sharedSecret, string friendlyName = "")
    {
        ClientIPAddress = clientIpAddress;
        SharedSecret = sharedSecret;
        FriendlyName = friendlyName;
    }

    [JsonPropertyName("ClientIPAddress")]
    [Name("ClientIPAddress")]
    public string ClientIPAddress { get; set; }

    [JsonPropertyName("SharedSecret")]
    [Name("SharedSecret")]
    public string SharedSecret { get; set; }

    [JsonPropertyName("FriendlyName")]
    [Name("FriendlyName")]
    public string FriendlyName { get; set; }
}
