using CommandLine;
using EZRadiusClient.Models;
using SampleApp.Managers;

namespace SampleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        RadiusAppManager radiusAppManager = new();
        Console.WriteLine("Welcome to the EZRadius Sample");
        int result = Parser
            .Default.ParseArguments<
                ShowPoliciesArgModel, 
                DownloadIPAddressArgModel, 
                UpdateIPAddressArgModel, 
                DeletePolicyArgModel, 
                GetAuditLogsArgModel
            >(args)
            .MapResult(
                (ShowPoliciesArgModel operation) => 
                    radiusAppManager.CallShowPoliciesAsync(operation).GetAwaiter().GetResult(), 
                (DownloadIPAddressArgModel operation) => 
                    radiusAppManager.CallDownloadIPAddressesAsync(operation).GetAwaiter().GetResult(), 
                (UpdateIPAddressArgModel operation) => 
                    radiusAppManager.CallUpdateIPAddressesAsync(operation).GetAwaiter().GetResult(),  
                (DeletePolicyArgModel operation) => 
                    radiusAppManager.CallDeleteRadiusPolicyAsync(operation).GetAwaiter().GetResult(), 
                (GetAuditLogsArgModel operation) => 
                    radiusAppManager.CallGetAuditLogsAsync(operation).GetAwaiter().GetResult(), 
                errs => radiusAppManager.ProcessErrors(errs));
        Environment.Exit(result);
    }
}


