using CommandLine;

namespace EZRadiusClient.Models;

[Verb("show", HelpText = "Display all policies in the EZRadius instance")]
public class ShowPoliciesArgModel
{
    [Option('s', "scope", Required = false, HelpText = "Token Scope to be used")]
    public string Scope { get; set; } = string.Empty;

    [Option(
        'u',
        "url",
        Required = false,
        HelpText = "URL for EZRadius Instance (Default: https://usa.ezradius.io"
    )]
    public string InstanceUrl { get; set; } = string.Empty;

    [Option(
        'l',
        "log",
        Required = false,
        HelpText = "Azure Application Insight connection string to send logs"
    )]
    public string AppInsightConnection { get; set; } = string.Empty;

    [Option(
        'a',
        "adUrl",
        Required = false,
        HelpText = "URL for the target Active Directory (Entra ID) instance"
    )]
    public string ADUrl { get; set; } = string.Empty;
}
