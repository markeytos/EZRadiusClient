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
        int result = await Parser
            .Default.ParseArguments<
                ShowPoliciesArgModel,
                DownloadIPAddressArgModel,
                UpdateIPAddressArgModel,
                DeletePolicyArgModel,
                GetAuditLogsArgModel
            >(args)
            .MapResult(
                async (ShowPoliciesArgModel operation) =>
                    await radiusAppManager.CallShowPoliciesAsync(operation),
                (DownloadIPAddressArgModel operation) =>
                    radiusAppManager.CallDownloadIPAddressesAsync(operation),
                async (UpdateIPAddressArgModel operation) =>
                    await radiusAppManager.CallUpdateIPAddressesAsync(operation),
                async (DeletePolicyArgModel operation) =>
                    await radiusAppManager.CallDeleteRadiusPolicyAsync(operation),
                async (GetAuditLogsArgModel operation) =>
                    await radiusAppManager.CallGetAuditLogsAsync(operation),
                errs => radiusAppManager.ProcessErrors(errs)
            );
        Environment.Exit(result);
    }
}
