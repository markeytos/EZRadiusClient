using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class AccessPolicyModel
{
    [JsonPropertyName("PolicyName")]
    public string PolicyName { get; set; } = string.Empty;

    [JsonPropertyName("PolicyID")]
    public string PolicyID { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("AccessPolicyID")]
    public string AccessPolicyID { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("SubscriptionID")]
    public string SubscriptionID { get; set; } = string.Empty;

    [JsonPropertyName("TenantID")]
    public string TenantID { get; set; } = string.Empty;

    [JsonPropertyName("VLANName")]
    public string VLANName { get; set; } = string.Empty;

    [JsonPropertyName("VLANCertificateAttribute")]
    public string VLANCertificateAttribute { get; set; } = string.Empty;

    [JsonPropertyName("VLANCertificateAttributePrefix")]
    public string VLANCertificateAttributePrefix { get; set; } = string.Empty;

    [JsonPropertyName("CheckOCSP")]
    public bool CheckOCSP { get; set; } = false;

    [JsonPropertyName("AuthorizationCertificateAttribute")]
    public string AuthorizationCertificateAttribute { get; set; } = string.Empty;

    [JsonPropertyName("AuthorizationCertificateAttributePrefix")]
    public string AuthorizationCertificateAttributePrefix { get; set; } = string.Empty;

    [JsonPropertyName("AuthorizationCertificateObjectType")]
    public string AuthorizationCertificateObjectType { get; set; } = string.Empty;

    [JsonPropertyName("IsUser")]
    public bool IsUser { get; set; } = false;

    [JsonPropertyName("MatchAADAttribute")]
    public bool MatchAADAttribute { get; set; } = false;

    [JsonPropertyName("AllowPasswords")]
    public bool AllowPasswords { get; set; } = false;

    [JsonPropertyName("UserFederation")]
    public bool UserFederation { get; set; } = false;

    [JsonPropertyName("PAP")]
    public bool PAP { get; set; } = false;

    [JsonPropertyName("EAPTTLS")]
    public bool EAPTTLS { get; set; } = false;

    [JsonPropertyName("IDP")]
    public string IDP { get; set; } = string.Empty;

    [JsonPropertyName("CheckGroupMembership")]
    public bool CheckGroupMembership { get; set; } = false;

    [JsonPropertyName("GroupID")]
    public string GroupID { get; set; } = string.Empty;

    [JsonPropertyName("RequiredEKUs")]
    public string RequiredEKUs { get; set; } = string.Empty;

    [JsonPropertyName("CheckDeviceHealth")]
    public bool CheckDeviceHealth { get; set; } = false;

    [JsonPropertyName("RulePriority")]
    public int RulePriority { get; set; } = 0;

    [JsonPropertyName("FilterId")]
    public string FilterId { get; set; } = string.Empty;
}
