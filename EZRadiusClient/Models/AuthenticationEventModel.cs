using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class AuthenticationEventModel
{
    [JsonPropertyName("SubscriptionId")]
    public string SubscriptionId { get; set; } = string.Empty;

    [JsonPropertyName("RadiusSubscriptionTenantId")]
    public string RadiusSubscriptionTenantId { get; set; } = string.Empty;

    [JsonPropertyName("PolicyID")]
    public string? PolicyID { get; set; }

    [JsonPropertyName("AccessPolicyId")]
    public string? AccessPolicyId { get; set; }

    [JsonPropertyName("AccessPolicyName")]
    public string? AccessPolicyName { get; set; }

    [JsonPropertyName("Successful")]
    public bool Successful { get; set; }

    [JsonPropertyName("Message")]
    public string? Message { get; set; }

    [JsonPropertyName("UserName")]
    public string? UserName { get; set; }

    [JsonPropertyName("CertificateThumbprint")]
    public string? CertificateThumbprint { get; set; }

    [JsonPropertyName("CertificateSubjectName")]
    public string? CertificateSubjectName { get; set; }

    [JsonPropertyName("Certificate")]
    public string? Certificate { get; set; }

    [JsonPropertyName("VLanName")]
    public string? VLanName { get; set; }

    [JsonPropertyName("FilterID")]
    public string? FilterID { get; set; }

    [JsonPropertyName("AuthenticationType")]
    public string? AuthenticationType { get; set; }

    [JsonPropertyName("DateCreated")]
    public DateTimeOffset DateCreated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("RequestingIP")]
    public string? RequestingIP { get; set; }

    [JsonPropertyName("RADIUSIP")]
    public string? RADIUSIP { get; set; }

    [JsonPropertyName("NasIpAddress")]
    public string? NasIpAddress { get; set; }

    [JsonPropertyName("NasIdentifier")]
    public string? NasIdentifier { get; set; }

    [JsonPropertyName("RadsecCertificate")]
    public string? RadsecCertificate { get; set; }
}
