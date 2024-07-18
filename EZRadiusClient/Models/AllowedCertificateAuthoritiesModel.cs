using System.Text.Json.Serialization;

namespace EZRadiusClient.Models;

public class AllowedCertificateAuthoritiesModel {
    public AllowedCertificateAuthoritiesModel() { }
    
    [JsonPropertyName("SubjectName")]
    public string SubjectName { get; set; } = string.Empty;
    
    [JsonPropertyName("CertificatePEM")]
    public string CertificatePEM { get; set; } = string.Empty;
    
    [JsonPropertyName("Thumbprint")]
    public string Thumbprint { get; set; } = string.Empty;
    
    [JsonPropertyName("ExpirationDate")]
    public DateTime ExpirationDate { get; set; }
    
    [JsonPropertyName("IsRootCA")]
    public bool IsRootCA { get; set; } = true;
    
    [JsonPropertyName("EZCA")]
    public bool EZCA { get; set; } = true;
    
    [JsonPropertyName("EZCACAID")]
    public string EZCACAID { get; set; } = string.Empty;
    
    [JsonPropertyName("EZCAWorkerID")]
    public string EZCAWorkerID { get; set; } = string.Empty;
    
    [JsonPropertyName("EZCALocation")]
    public string EZCALocation { get; set; } = RadiusConstants.EZCAGlobal;
}