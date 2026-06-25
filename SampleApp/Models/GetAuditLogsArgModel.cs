using CommandLine;

namespace EZRadiusClient.Models;

[Verb("getlogs", HelpText = "Get Authorization Audit logs from EZRadius Instance")]
public class GetAuditLogsArgModel
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

    [Option('d', "days", Required = false, HelpText = "Number of days to fetch logs for")]
    public int? Days { get; set; }

    [Option(
        "from",
        Required = false,
        HelpText = "Start date/time for log range (for example: 2026-01-01 or 2026-01-01T00:00:00)"
    )]
    public string? DateFrom { get; set; }

    [Option(
        "to",
        Required = false,
        HelpText = "End date/time for log range (for example: 2026-01-31 or 2026-01-31T23:59:59)"
    )]
    public string? DateTo { get; set; }

    [Option(
        'f',
        "file",
        Required = false,
        HelpText = "File path to save logs to (must end in .csv)"
    )]
    public string? FileName { get; set; }
}
