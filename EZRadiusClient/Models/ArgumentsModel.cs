namespace EZRadiusClient.Models;

public class ArgumentsModel
{
    public ArgumentsModel(DeletePolicyArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
    }

    public ArgumentsModel(DownloadIPAddressArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
        csvFilePath = arguments.OutputFilePath;
    }

    public ArgumentsModel(UpdateIPAddressArgModel arguments)
    {
        Scope = arguments.Scope;
        InstanceUrl = arguments.InstanceUrl;
        ADUrl = arguments.ADUrl;
        AppInsightConnection = arguments.AppInsightConnection;
        csvFilePath = arguments.InputFilePath;
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
    }
    
    public string Scope { get; set; }
    
    public string InstanceUrl { get; set; }
    
    public string ADUrl { get; set; }
    
    public string AppInsightConnection { get; set; }

    public string? csvFilePath { get; set; } = string.Empty;
}