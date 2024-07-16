using System.ComponentModel.DataAnnotations;

namespace EZRadiusClient.Models;

public class RadiusPolicyModel
    {
        public RadiusPolicyModel() {}
        
        [Required]
        public string PolicyID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(128)]
        public string SubscriptionID { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string TenantID { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string PolicyName { get; set; } = string.Empty;

        public string? ServerCertificatePEM { get; set; }
        public string? IntermidiateCertificatePEM { get; set; }
        public string? RootCACertificatePEM { get; set; }

        public string? ServerCertificateKeyLocation { get; set; }

        public string? PendingCSR { get; set; } = string.Empty;

        public bool EZCAEnabled { get; set; } = true;

        public string? EZCAInstanceForCertificate { get; set; } = string.Empty;

        public string? EZCACAID { get; set; }

        public string? EZCATemplateID { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public RadiusAlgorithmEnum EnabledRADIUSProtocols { get; set; } = RadiusAlgorithmEnum.EAPTLS;
            
    }