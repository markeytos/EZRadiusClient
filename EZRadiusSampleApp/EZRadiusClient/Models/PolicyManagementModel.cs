using System.Text.Json.Serialization;

namespace EZRadiusClient.Models
{
    public class PolicyManagementModel
    {
        public PolicyManagementModel() { }
        
        [JsonPropertyName("RadiusServers")]
        public List<RadiusServerDetails> RadiusServers { get; set; } = new();

        [JsonPropertyName("AvailableSubscriptions")]
        public List<string> AvailableSubscriptions { get; set; } = new();

        [JsonPropertyName("RadiusPolicies")]
        public List<RadiusPolicyModel> RadiusPolicies { get; set; } = new();
    }
}