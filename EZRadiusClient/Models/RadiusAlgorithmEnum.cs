namespace EZRadiusClient.Models;

[Flags]
public enum RadiusAlgorithmEnum
{
    None = 0,
    EAPTLS = 1,
    EAPTTLS = 2,
    PAP = 4,
}
