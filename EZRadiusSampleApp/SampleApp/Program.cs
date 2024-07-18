using System.Globalization;
using EZRadiusClient.Managers;
using Azure.Identity;
using CsvHelper;
using CommandLine;
using CsvHelper.Configuration;
using EZRadiusClient.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

string appInsightsConnectionString = "";
string adInstanceUrl = "";
string instanceUrl = "";
string scopes = "";
string csvFilePath = "";

Console.WriteLine("Welcome to the EZRadius Sample");
int result = Parser.Default.ParseArguments<ArgumentsModel>(args).MapResult((opts) => InitializeVariables(opts), errs => ProcessErrors(errs));
if (result != 0)
{
    Console.WriteLine("Error parsing arguments, please try again. Use --help or check documentation for more information.");
    return;
}

if (string.IsNullOrWhiteSpace(adInstanceUrl))
{
    adInstanceUrl = "https://login.microsoftonline.com/";
}
if (string.IsNullOrWhiteSpace(instanceUrl))
{
    instanceUrl = "https://usa.ezradius.io/";
}
if (string.IsNullOrWhiteSpace(scopes))
{
    scopes = "5c0e7b30-d0aa-456a-befb-df8c75e8467b/.default";
}

ILogger logger = CreateLogger(appInsightsConnectionString);
var cliAuthentication = new AzureCliCredentialOptions { AuthorityHost = new Uri(adInstanceUrl) };
IEZRadiusManager ezRadiusClient = new EZRadiusManager(new HttpClient(), logger, new AzureCliCredential(cliAuthentication), instanceUrl, scopes);

try
{
    Console.WriteLine("Getting current Radius Policies");
    List<RadiusPolicyModel> currentRadiusPolicies = await ezRadiusClient.GetRadiusPoliciesAsync();
    Console.WriteLine($"Found {currentRadiusPolicies.Count} policies");
    
    Console.WriteLine("Grabbing IP Addresses from current policy and saving to CSV file");
    APIResultModel getIPAddressesResult = GetAllowedIPAddressesInCSVAsync(csvFilePath, currentRadiusPolicies[0]);
    Console.WriteLine(getIPAddressesResult.Message);
    
    Console.WriteLine("Updating Policy IP Addresses from CSV file");
    APIResultModel updateIPAddressesResult = await UpdateIPAddressesWithCSVAsync(csvFilePath, currentRadiusPolicies[0]);
    Console.WriteLine(updateIPAddressesResult.Message);
}
catch (Exception e)
{
    logger.LogError(e, "Error occured while getting or updating policy");
    Console.WriteLine(e);
}

async Task<APIResultModel> UpdateIPAddressesWithCSVAsync(string pathToCSVFile, RadiusPolicyModel currentPolicy)
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
        return await ezRadiusClient.EditRadiusPolicyAsync(currentPolicy);
    }
    catch (Exception e)
    {
        return new APIResultModel(false, e.Message);
    }
}

APIResultModel GetAllowedIPAddressesInCSVAsync(string pathToCSVFile, RadiusPolicyModel currentPolicy)
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
        return new APIResultModel(false, e.Message);
    }
}

int InitializeVariables(ArgumentsModel opts)
{
    appInsightsConnectionString = opts.AppInsight;
    adInstanceUrl = opts.ADUrl;
    instanceUrl = opts.InstanceUrl;
    scopes = opts.Scope;
    csvFilePath = opts.csvFilePath;
    return 0;
}

int ProcessErrors(IEnumerable<Error> errs)
{
    Console.WriteLine(errs);
    return 1;
}

ILogger CreateLogger(string? appInsightsKey)
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