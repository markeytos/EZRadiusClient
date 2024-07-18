using CommandLine;

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
    
    public string Scope { get; set; } = string.Empty;
    
    public string InstanceUrl { get; set; } = string.Empty;
    
    public string ADUrl { get; set; } = string.Empty;
    
    public string AppInsightConnection { get; set; } = string.Empty;

    public string csvFilePath { get; set; } = string.Empty;
}