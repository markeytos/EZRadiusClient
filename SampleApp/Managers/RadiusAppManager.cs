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
    public Task<int> ProcessErrors(IEnumerable<Error> errs)
    {
        Console.WriteLine(errs);
        return Task.FromResult(1);
    }

    public async Task<int> CallShowPoliciesAsync(ShowPoliciesArgModel passedArguments)
    {
        ArgumentsModel parameters = new(passedArguments);
        parameters = InputService.ValidateArguments(parameters);
        IEZRadiusManager ezRadiusClient = InitializeEzRadiusManager(parameters);

        try
        {
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(
                ezRadiusClient
            );
            foreach (RadiusPolicyModel radiusPolicy in currentRadiusPolicies)
            {
                Console.WriteLine($"=== {radiusPolicy.PolicyName} ===");
                Console.WriteLine($"Policy ID: {radiusPolicy.PolicyID}");
                Console.WriteLine($"Subscription ID: {radiusPolicy.SubscriptionID}");
                Console.WriteLine($"Allowed IP Addresses: {radiusPolicy.AllowedIPAddresses.Count}");
                foreach (AllowedIPAddressModel allowedIPAddress in radiusPolicy.AllowedIPAddresses)
                {
                    string friendlyNameDisplay = string.IsNullOrWhiteSpace(allowedIPAddress.FriendlyName) 
                        ? "" 
                        : $" ({allowedIPAddress.FriendlyName})";
                    Console.WriteLine($"\t {allowedIPAddress.ClientIPAddress}{friendlyNameDisplay}");
                }

                Console.WriteLine($"Access Policies: {radiusPolicy.AccessPolicies.Count}");
                foreach (AccessPolicyModel accessPolicy in radiusPolicy.AccessPolicies)
                {
                    Console.WriteLine($"\t {accessPolicy.PolicyName}");
                }

                Console.WriteLine(
                    $"Allowed Certificate Authorities: {radiusPolicy.AllowedCertificateAuthorities.Count}"
                );
                foreach (
                    AllowedCertificateAuthoritiesModel allowedCA in radiusPolicy.AllowedCertificateAuthorities
                )
                {
                    Console.WriteLine($"\t {allowedCA.SubjectName}");
                }

                Console.WriteLine(
                    $"Server Certificate Authority: {radiusPolicy.ServerCertificate.SubjectName}"
                );
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

        if (string.IsNullOrWhiteSpace(parameters.PolicyName))
        {
            Console.WriteLine(
                "Missing Policy Name argument. Please provide a policy name to download IP addresses from"
            );
            return 1;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(parameters.CsvFilePath))
            {
                throw new Exception("Please provide a path to save the IP addresses");
            }
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(
                ezRadiusClient
            );
            RadiusPolicyModel? chosenPolicyModel = currentRadiusPolicies.FirstOrDefault(policy =>
                policy.PolicyName == parameters.PolicyName
            );
            if (chosenPolicyModel == null)
            {
                Console.WriteLine("Policy not found");
                return 1;
            }
            Console.WriteLine(
                "Grabbing IP Addresses from "
                    + chosenPolicyModel.PolicyName
                    + " and saving them to CSV file"
            );
            APIResultModel getIPAddressesResult = PutAllowedIPAddressesInCSVAsync(
                parameters.CsvFilePath,
                chosenPolicyModel
            );
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

        if (string.IsNullOrWhiteSpace(parameters.PolicyName))
        {
            Console.WriteLine(
                "Missing Policy Name argument. Please provide a policy name to upload IP addresses to"
            );
            return 1;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(parameters.CsvFilePath))
            {
                throw new Exception("Please provide a path to save the IP addresses");
            }
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(
                ezRadiusClient
            );
            RadiusPolicyModel? chosenPolicyModel = currentRadiusPolicies.FirstOrDefault(policy =>
                policy.PolicyName == parameters.PolicyName
            );
            if (chosenPolicyModel == null)
            {
                Console.WriteLine("Policy not found");
                return 1;
            }
            APIResultModel updateIPAddressesResult = await UpdateIPAddressesWithCSVAsync(
                parameters.CsvFilePath,
                chosenPolicyModel,
                ezRadiusClient
            );
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

        if (string.IsNullOrWhiteSpace(parameters.PolicyName))
        {
            Console.WriteLine(
                "Missing Policy Name argument. Please provide a policy name to delete"
            );
            return 1;
        }

        try
        {
            List<RadiusPolicyModel> currentRadiusPolicies = await GetRadiusPoliciesAsync(
                ezRadiusClient
            );
            RadiusPolicyModel? chosenPolicyModel = currentRadiusPolicies.FirstOrDefault(policy =>
                policy.PolicyName == parameters.PolicyName
            );
            if (chosenPolicyModel == null)
            {
                Console.WriteLine("Policy not found");
                return 1;
            }
            Console.WriteLine("Deleting " + chosenPolicyModel.PolicyName);
            APIResultModel deletePolicyResult = await ezRadiusClient.DeleteRadiusPolicyAsync(
                chosenPolicyModel
            );
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

        if (!TryBuildTimeFrame(parameters, out TimeFrameModel? timeFrame, out string description, out string validationError))
        {
            Console.WriteLine(validationError);
            return 1;
        }

        try
        {
            Console.WriteLine($"Getting Authentication Audit Logs for {description}...");
            List<AuthenticationEventModel> getAuditLogsResult =
                await ezRadiusClient.GetAuthAuditLogsAsync(timeFrame);
            Console.WriteLine($"Found {getAuditLogsResult.Count} logs");
            if (!string.IsNullOrWhiteSpace(parameters.CsvFilePath))
            {
                await using (StreamWriter writer = new(parameters.CsvFilePath))
                {
                    await using (
                        CsvWriter csvWriter =
                            new(writer, new CsvConfiguration(CultureInfo.InvariantCulture))
                    )
                    {
                        await csvWriter.WriteRecordsAsync(getAuditLogsResult);
                    }
                }
            }
            else
            {
                foreach (AuthenticationEventModel authenticationEventLog in getAuditLogsResult)
                {
                    Console.WriteLine($"=== {authenticationEventLog.DateCreated} ===");
                    Console.WriteLine($"User: {authenticationEventLog.UserName}");
                    Console.WriteLine($"Radius IP: {authenticationEventLog.RADIUSIP}");
                    Console.WriteLine($"Requesting IP: {authenticationEventLog.RequestingIP}");
                    Console.WriteLine(
                        $"Authentication Type: {authenticationEventLog.AuthenticationType}"
                    );
                    Console.WriteLine(
                        $"Access Policy Name: {authenticationEventLog.AccessPolicyName}"
                    );
                    Console.WriteLine($"Message: {authenticationEventLog.Message}");
                    Console.WriteLine($"Successful: {authenticationEventLog.Successful}");
                }
            }
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
    }

    private static bool TryBuildTimeFrame(
        ArgumentsModel parameters,
        out TimeFrameModel? timeFrame,
        out string description,
        out string validationError
    )
    {
        timeFrame = null;
        description = string.Empty;
        validationError = string.Empty;

        bool hasDays = parameters.Days.HasValue;
        bool hasFrom = !string.IsNullOrWhiteSpace(parameters.DateFrom);
        bool hasTo = !string.IsNullOrWhiteSpace(parameters.DateTo);

        if (hasDays && (hasFrom || hasTo))
        {
            validationError = "Use either --days or --from/--to, not both.";
            return false;
        }

        if (hasDays)
        {
            if (parameters.Days <= 0)
            {
                validationError = "--days must be greater than 0.";
                return false;
            }

            timeFrame = new TimeFrameModel(parameters.Days.Value);
            description = $"the past {parameters.Days.Value} days";
            return true;
        }

        if (hasFrom ^ hasTo)
        {
            validationError = "When using a date range, both --from and --to are required.";
            return false;
        }

        if (!(hasFrom && hasTo))
        {
            validationError = "Provide either --days <n> or --from <date> --to <date>.";
            return false;
        }

        if (
            !DateTimeOffset.TryParse(
                parameters.DateFrom,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset parsedFrom
            )
        )
        {
            validationError = "Invalid --from value. Use a valid date/time such as 2026-01-01 or 2026-01-01T00:00:00.";
            return false;
        }

        if (
            !DateTimeOffset.TryParse(
                parameters.DateTo,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset parsedTo
            )
        )
        {
            validationError = "Invalid --to value. Use a valid date/time such as 2026-01-31 or 2026-01-31T23:59:59.";
            return false;
        }

        if (parsedFrom > parsedTo)
        {
            validationError = "--from must be earlier than or equal to --to.";
            return false;
        }

        timeFrame = new TimeFrameModel(parsedFrom.UtcDateTime, parsedTo.UtcDateTime);
        description = $"{parsedFrom.UtcDateTime:u} to {parsedTo.UtcDateTime:u}";
        return true;
    }

    private async Task<List<RadiusPolicyModel>> GetRadiusPoliciesAsync(
        IEZRadiusManager ezRadiusClient
    )
    {
        try
        {
            Console.WriteLine("Getting current Radius policies...");
            List<RadiusPolicyModel> currentRadiusPolicies =
                await ezRadiusClient.GetRadiusPoliciesAsync();
            Console.WriteLine($"Found {currentRadiusPolicies.Count} policies");
            return currentRadiusPolicies;
        }
        catch (Exception e)
        {
            throw new Exception("Error getting current radius policies" + e.Message);
        }
    }

    private async Task<APIResultModel> UpdateIPAddressesWithCSVAsync(
        string pathToCSVFile,
        RadiusPolicyModel currentPolicy,
        IEZRadiusManager ezRadiusClient
    )
    {
        try
        {
            List<AllowedIPAddressModel> allowedIPAddresses = new();
            var reader = new StreamReader(pathToCSVFile);
            var csvReader = new CsvReader(
                reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
            );
            var records = csvReader.GetRecords<AllowedIPAddressModel>().ToList();
            foreach (var record in records)
            {
                allowedIPAddresses.Add(
                    new AllowedIPAddressModel(record.ClientIPAddress, record.SharedSecret, record.FriendlyName)
                );
            }
            currentPolicy.AllowedIPAddresses = allowedIPAddresses;
            return await ezRadiusClient.CreateOrEditRadiusPolicyAsync(currentPolicy);
        }
        catch (Exception e)
        {
            throw new Exception("Error updating IP addresses" + e.Message);
        }
    }

    private APIResultModel PutAllowedIPAddressesInCSVAsync(
        string pathToCSVFile,
        RadiusPolicyModel currentPolicy
    )
    {
        try
        {
            List<AllowedIPAddressModel> allowedIPAddresses = currentPolicy.AllowedIPAddresses;
            using (var writer = new StreamWriter(pathToCSVFile))
            using (
                var csvWriter = new CsvWriter(
                    writer,
                    new CsvConfiguration(CultureInfo.InvariantCulture)
                )
            )
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
        var cliAuthentication = new AzureCliCredentialOptions
        {
            AuthorityHost = new Uri(passedArguments.ADUrl)
        };
        IEZRadiusManager ezRadiusClient = new EZRadiusManager(
            new HttpClient(),
            logger,
            new AzureCliCredential(cliAuthentication),
            passedArguments.InstanceUrl,
            passedArguments.Scope
        );
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
