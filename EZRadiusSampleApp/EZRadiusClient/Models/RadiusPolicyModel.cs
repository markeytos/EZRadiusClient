using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class RadiusPolicyModel
{
    public RadiusPolicyModel() { }
        
        [JsonPropertyName("PolicyName")]
        public string PolicyName { get; set; } = string.Empty;

        [JsonPropertyName("PolicyID")]
        public string PolicyID { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("SubscriptionID")]
        public string SubscriptionID { get; set; } = string.Empty;

        [JsonPropertyName("AllowedIPAddresses")]
        public List<AllowedIPAddressModel> AllowedIPAddresses { get; set; } = new();

        [JsonPropertyName("AllowedCertificateAuthorities")]
        public List<AllowedCertificateAuthoritiesModel> AllowedCertificateAuthorities { get; set; } =
            new();

        [JsonPropertyName("ServerCA")]
        public ServerCAModel ServerCertificate { get; set; } = new();

        [JsonPropertyName("AccessPolicies")] 
        public List<DBAccessPolicyModel> AccessPolicies { get; set; } = new();
}