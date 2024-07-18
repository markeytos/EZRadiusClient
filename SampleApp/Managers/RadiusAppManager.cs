using System.Globalization;
using Azure.Identity;
using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using EZRadiusClient.Managers;
using EZRadiusClient.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleApp.Services;

namespace SampleApp.Managers;

public class RadiusAppManager
{
    public int ProcessErrors(IEnumerable<Error> errs)
    {
        Console.WriteLine(errs);
        return 1;
    }
    
    public async Task<int> CallShowPoliciesAsync(ShowPoliciesArgModel passedArguments)
    {
        ArgumentsModel parameters = new(passedArguments);
        parameters = InputService.ValidateArguments(parameters);
        IEZRadiusManager ezRadiusClient = InitializeEzRadiusManager(parameters);

        try
        {
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(ezRadiusClient);
            foreach (RadiusPolicyModel radiusPolicy in currentRadiusPolicies)
            {
                Console.WriteLine($"=== {radiusPolicy.PolicyName} ===");
                Console.WriteLine($"Policy ID: {radiusPolicy.PolicyID}");
                Console.WriteLine($"Subscription ID: {radiusPolicy.SubscriptionID}");
                Console.WriteLine($"Allowed IP Addresses: {radiusPolicy.AllowedIPAddresses.Count}");
                foreach (AllowedIPAddressModel allowedIPAddress in radiusPolicy.AllowedIPAddresses)
                {
                    Console.WriteLine($"\t {allowedIPAddress.ClientIPAddress}");
                }

                Console.WriteLine($"Access Policies: {radiusPolicy.AccessPolicies.Count}");
                foreach (AccessPolicyModel accessPolicy in radiusPolicy.AccessPolicies)
                {
                    Console.WriteLine($"\t {accessPolicy.PolicyName}");
                }

                Console.WriteLine(
                    $"Allowed Certificate Authorities: {radiusPolicy.AllowedCertificateAuthorities.Count}");
                foreach (AllowedCertificateAuthoritiesModel allowedCA in radiusPolicy.AllowedCertificateAuthorities)
                {
                    Console.WriteLine($"\t {allowedCA.SubjectName}");
                }

                Console.WriteLine($"Server Certificate Authority: {radiusPolicy.ServerCertificate.SubjectName}");
            }

            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
    }
    
    public async Task<int> CallDownloadIPAddressesAsync(DownloadIPAddressArgModel passedArguments)
    {
        ArgumentsModel parameters = new(passedArguments);
        parameters = InputService.ValidateArguments(parameters);
        IEZRadiusManager ezRadiusClient = InitializeEzRadiusManager(parameters);

        try
        {
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(ezRadiusClient);
            int chosenPolicyIndex =
                InputService.ChooseRadiusPolicy(currentRadiusPolicies, "download IP addresses from");
            Console.WriteLine("Grabbing IP Addresses from " + currentRadiusPolicies[chosenPolicyIndex].PolicyName +
                              " and saving them to CSV file");
            APIResultModel getIPAddressesResult =
                PutAllowedIPAddressesInCSVAsync(parameters.csvFilePath, currentRadiusPolicies[chosenPolicyIndex]);
            Console.WriteLine(getIPAddressesResult.Message);
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
    }
    
    public async Task<int> CallUpdateIPAddressesAsync(UpdateIPAddressArgModel passedArguments)
    {
        ArgumentsModel parameters = new(passedArguments);
        parameters = InputService.ValidateArguments(parameters);
        IEZRadiusManager ezRadiusClient = InitializeEzRadiusManager(parameters);

        try
        {
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(ezRadiusClient);
            int chosenPolicyIndex = InputService.ChooseRadiusPolicy(currentRadiusPolicies, "upload IP addresses to");
            Console.WriteLine("Updating IP Addresses for " + currentRadiusPolicies[chosenPolicyIndex].PolicyName +
                              " from CSV file");
            APIResultModel updateIPAddressesResult = await UpdateIPAddressesWithCSVAsync(parameters.csvFilePath,
                currentRadiusPolicies[chosenPolicyIndex], ezRadiusClient);
            Console.WriteLine(updateIPAddressesResult.Message);
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
    }
    
    public async Task<int> CallDeleteRadiusPolicyAsync(DeletePolicyArgModel passedArguments)
    {
        ArgumentsModel parameters = new(passedArguments);
        parameters = InputService.ValidateArguments(parameters);
        IEZRadiusManager ezRadiusClient = InitializeEzRadiusManager(parameters);

        try
        {
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(ezRadiusClient);
            int chosenPolicyIndex = InputService.ChooseRadiusPolicy(currentRadiusPolicies, "delete");
            Console.WriteLine("Deleting " + currentRadiusPolicies[chosenPolicyIndex].PolicyName);
            APIResultModel deletePolicyResult =
                await ezRadiusClient.DeleteRadiusPolicyAsync(currentRadiusPolicies[chosenPolicyIndex]);
            Console.WriteLine(deletePolicyResult.Message);
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }

    }
    
    public async Task<int> CallGetAuditLogsAsync(GetAuditLogsArgModel passedArguments)
    {
        ArgumentsModel parameters = new(passedArguments);
        parameters = InputService.ValidateArguments(parameters);
        IEZRadiusManager ezRadiusClient = InitializeEzRadiusManager(parameters);
        
        TimeFrameModel timeFrame = new();

        try
        {
            Console.WriteLine("Getting Authentication Audit Logs for past 2 days...");
            List<AuthenticationEventModel> getAuditLogsResult = await ezRadiusClient.GetAuthAuditLogsAsync(timeFrame);
            Console.WriteLine($"Found {getAuditLogsResult.Count} logs");
            foreach (AuthenticationEventModel authenticationEventLog in getAuditLogsResult)
            {
                Console.WriteLine($"=== {authenticationEventLog.DateCreated} ===");
                Console.WriteLine($"User: {authenticationEventLog.UserName}");
                Console.WriteLine($"Radius IP: {authenticationEventLog.RADIUSIP}");
                Console.WriteLine($"Requesting IP: {authenticationEventLog.RequestingIP}");
                Console.WriteLine($"Authentication Type: {authenticationEventLog.AuthenticationType}");
                Console.WriteLine($"Access Policy Name: {authenticationEventLog.AccessPolicyName}");
                Console.WriteLine($"Message: {authenticationEventLog.Message}");
                Console.WriteLine($"Successful: {authenticationEventLog.Successful}");
            }
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
    }
    
    private async Task<List<RadiusPolicyModel>> GetRadiusPoliciesAsync(IEZRadiusManager ezRadiusClient)
    {
        try
        {
            Console.WriteLine("Getting current Radius policies...");
            List<RadiusPolicyModel> currentRadiusPolicies = await ezRadiusClient.GetRadiusPoliciesAsync();
            Console.WriteLine($"Found {currentRadiusPolicies.Count} policies");
            return currentRadiusPolicies;
        }
        catch (Exception e)
        {
            throw new Exception("Error getting current radius policies" + e.Message);
        }
    }
    
    private async Task<APIResultModel> UpdateIPAddressesWithCSVAsync(string pathToCSVFile, RadiusPolicyModel currentPolicy, IEZRadiusManager ezRadiusClient)
    {
        try
        {
            List<AllowedIPAddressModel> allowedIPAddresses = new();
            var reader = new StreamReader(pathToCSVFile);
            var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            var records = csvReader.GetRecords<AllowedIPAddressModel>().ToList();
            foreach (var record in records)
            {
                allowedIPAddresses.Add(new AllowedIPAddressModel(record.ClientIPAddress, record.SharedSecret));
            }
            currentPolicy.AllowedIPAddresses = allowedIPAddresses;
            return await ezRadiusClient.CreateOrEditRadiusPolicyAsync(currentPolicy);
        }
        catch (Exception e)
        {
            throw new Exception("Error updating IP addresses" + e.Message);
        }
    }
    
    private APIResultModel PutAllowedIPAddressesInCSVAsync(string pathToCSVFile, RadiusPolicyModel currentPolicy)
    {
        try
        {
            List<AllowedIPAddressModel> allowedIPAddresses = currentPolicy.AllowedIPAddresses;
            using (var writer = new StreamWriter(pathToCSVFile)) 
            using (var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csvWriter.WriteRecords(allowedIPAddresses);
            }
            return new APIResultModel(true, "IP Addresses saved to " + pathToCSVFile);
        }
        catch (Exception e)
        {
            throw new Exception("Error saving IP addresses to CSV" + e.Message);
        }
    }
    
    private IEZRadiusManager InitializeEzRadiusManager(ArgumentsModel passedArguments)
    {
        ILogger logger = CreateLogger(passedArguments.AppInsightConnection);
        var cliAuthentication = new AzureCliCredentialOptions { AuthorityHost = new Uri(passedArguments.ADUrl) };
        IEZRadiusManager ezRadiusClient = new EZRadiusManager(new HttpClient(), logger, new AzureCliCredential(cliAuthentication), passedArguments.InstanceUrl, passedArguments.Scope);
        return ezRadiusClient;
    }
    
    private ILogger CreateLogger(string? appInsightsKey)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            if (!string.IsNullOrWhiteSpace(appInsightsKey))
            {
                builder.AddApplicationInsights(
                    configureTelemetryConfiguration: (config) =>
                        config.ConnectionString = appInsightsKey,
                    configureApplicationInsightsLoggerOptions: (_) => { }
                );
            }
#if WINDOWS
        builder.AddEventLog();
#endif
        });
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ILogger<Program>>();
    }
}