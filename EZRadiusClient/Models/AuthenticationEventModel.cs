using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class AuthenticationEventModel
{
    [JsonPropertyName("SubscriptionId")]
    public string? SubscriptionId { get; set; }

    [JsonPropertyName("RadiusSubscriptionTenantId")]
    public string? RadiusSubscriptionTenantId { get; set; }

    [JsonPropertyName("PolicyID")]
    public string? PolicyID { get; set; }

    [JsonPropertyName("AccessPolicyId")]
    public string? AccessPolicyId { get; set; }

    [JsonPropertyName("AccessPolicyName")]
    public string? AccessPolicyName { get; set; }

    [JsonPropertyName("Successful")]
    public bool Successful { get; set; }

    [JsonPropertyName("Message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("UserName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("CertificateThumbprint")]
    public string CertificateThumbprint { get; set; } = string.Empty;

    [JsonPropertyName("Certificate")]
    public string Certificate { get; set; } = string.Empty;

    [JsonPropertyName("VLanName")]
    public string VLanName { get; set; } = string.Empty;

    [JsonPropertyName("FilterID")]
    public string FilterID { get; set; } = string.Empty;

    [JsonPropertyName("AuthenticationType")]
    public string AuthenticationType { get; set; } = string.Empty;

    [JsonPropertyName("DateCreated")]
    public DateTimeOffset DateCreated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("RequestingIP")]
    public string RequestingIP { get; set; } = string.Empty;

    [JsonPropertyName("RADIUSIP")]
    public string RADIUSIP { get; set; } = string.Empty;
}
