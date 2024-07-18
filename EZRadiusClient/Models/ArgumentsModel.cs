using CommandLine;

namespace EZRadiusClient.Models;

public class ArgumentsModel
{
    [Option('s',"scope", Required = false, HelpText = "Scope to use")]
    public string Scope { get; set; } = string.Empty;
    
    [Option('u',"url", Required = false, HelpText = "URL for the target EZRadius instance")]
    public string InstanceUrl { get; set; } = string.Empty;
    
    [Option('a', "adUrl", Required = false, HelpText = "URL for the target Active Directory instance")]
    public string ADUrl { get; set; } = string.Empty;
    
    [Option('i',"insight", Required = false, HelpText = "Insight connection string for the logger")]
    public string AppInsight { get; set; } = string.Empty;

    [Option('o', "file", Required = false, HelpText = "Path to a CSV file to store and get IP addresses")]
    public string csvFilePath { get; set; } = string.Empty;
}