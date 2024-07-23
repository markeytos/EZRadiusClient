namespace EZRadiusClient.Models;

public class ArgumentsModel
{
    public ArgumentsModel(DeletePolicyArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
        PolicyName = arguments.PolicyName;
    }

    public ArgumentsModel(DownloadIPAddressArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
        CsvFilePath = arguments.OutputFilePath;
        PolicyName = arguments.PolicyName;
    }

    public ArgumentsModel(UpdateIPAddressArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
        CsvFilePath = arguments.InputFilePath;
        PolicyName = arguments.PolicyName;
    }

    public ArgumentsModel(ShowPoliciesArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
    }

    public ArgumentsModel(GetAuditLogsArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
        Days = arguments.Days;
    }

    public string Scope { get; set; }

    public string InstanceUrl { get; set; }

    public string ADUrl { get; set; }

    public string AppInsightConnection { get; set; }

    public string? CsvFilePath { get; set; }

    public int? Days { get; set; }

    public string? PolicyName { get; set; }
}
