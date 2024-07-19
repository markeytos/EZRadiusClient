using CommandLine;

namespace EZRadiusClient.Models;

[Verb("upload", HelpText = "Upload IP addresses to Radius policy from a CSV file")]
public class UpdateIPAddressArgModel
{
    [Option('s', "scope", Required = false, HelpText = "Token Scope to be used")] 
    public string Scope { get; set; } = string.Empty;
    
    [Option('i', "input", Required = true, HelpText = "Required. Path to a CSV file with IP addresses to upload")]
    public string InputFilePath { get; set; } = string.Empty;
    
    [Option('u', "url", Required = false, HelpText = "URL for EZRadius Instance (Default: https://usa.ezradius.io")]
    public string InstanceUrl { get; set; } = string.Empty;

    [Option('l', "log", Required = false, HelpText = "Azure Application Insight connection string to send logs")]
    public string AppInsightConnection { get; set; } = string.Empty;

    [Option('a', "adUrl", Required = false, HelpText = "URL for the target Active Directory (Entra ID) instance")]
    public string ADUrl { get; set; } = string.Empty;
    
    [Option('n', "name", Required = true, HelpText = "Required. Name of the Radius policy to download IP addresses from")]
    public string PolicyName { get; set; } = string.Empty;
}