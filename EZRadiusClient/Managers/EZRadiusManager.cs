using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using EZRadiusClient.Models;
using EZRadiusClient.Services;
using Microsoft.Extensions.Logging;

namespace EZRadiusClient.Managers;

public interface IEZRadiusManager
{
    /// <summary>
    /// Gets all the current Radius policies from the EZRadius instance database
    /// </summary>
    /// <returns>List of <see cref="RadiusPolicyModel"/></returns> Radius Policies
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<List<RadiusPolicyModel>> GetRadiusPoliciesAsync();

    /// <summary>
    /// Creates a new Radius policy or overwrites existing Radius policy in the EZRadius database with passed policy
    /// </summary>
    /// <param name="policy"><see cref="RadiusPolicyModel"/> to be created or edited</param>
    /// <returns><see cref="APIResultModel"/> with success bool and results or error message</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<APIResultModel> CreateOrEditRadiusPolicyAsync(RadiusPolicyModel policy);

    /// <summary>
    /// Deletes an existing Radius policy in the EZRadius Database with passed policy
    /// </summary>
    /// <param name="policy"><see cref="RadiusPolicyModel"/> to be created or edited</param>
    /// <returns><see cref="APIResultModel"/> with success bool and message</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<APIResultModel> DeleteRadiusPolicyAsync(RadiusPolicyModel policy);

    /// <summary>
    /// Gets the Authorization Audit logs for the passed time frame
    /// </summary>
    /// <param name="timeFrame"><see cref="TimeFrameModel"/> containing start and end date</param>
    /// <returns>List of <see cref="AuthenticationEventModel"/></returns> containing logging information
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<List<AuthenticationEventModel>> GetAuthAuditLogsAsync(TimeFrameModel timeFrame);
}

public class EZRadiusManager : IEZRadiusManager
{
    private readonly HttpClientService _httpClient;
    private readonly string _url;
    private AccessToken _token;
    private readonly TokenCredential? _azureTokenCredential;
    private readonly string _scopes;

    public EZRadiusManager(
        HttpClient httpClient,
        ILogger? logger = null,
        TokenCredential? azureTokenCredential = null,
        string baseUrl = "https://usa.ezradius.io/",
        string scopes = "https://management.core.windows.net/.default"
    )
    {
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl));
        }
        if (azureTokenCredential == null)
        {
            _azureTokenCredential = new DefaultAzureCredential();
        }
        else
        {
            _azureTokenCredential = azureTokenCredential;
        }
        _httpClient = new HttpClientService(httpClient, logger);
        _url = baseUrl.TrimEnd('/').Replace("http://", "https://");
        _scopes = scopes;
    }

    public async Task<List<RadiusPolicyModel>> GetRadiusPoliciesAsync()
    {
        await GetTokenAsync();
        APIResultModel getRadiusPoliciesResponse = await _httpClient.CallGenericAsync(
            _url + "/api/Policies/GetRadiusPolicies",
            null,
            _token.Token,
            HttpMethod.Get
        );
        if (getRadiusPoliciesResponse.Success && getRadiusPoliciesResponse.Message != null)
        {
            PolicyManagementModel? currentPolicyManagementModel =
                JsonSerializer.Deserialize<PolicyManagementModel>(
                    getRadiusPoliciesResponse.Message
                );
            if (currentPolicyManagementModel == null)
            {
                throw new HttpRequestException("Error deserializing current radius policies");
            }
            return currentPolicyManagementModel.RadiusPolicies;
        }
        else
        {
            throw new HttpRequestException(
                "Error getting current radius policies" + getRadiusPoliciesResponse.Message
            );
        }
    }

    public async Task<APIResultModel> CreateOrEditRadiusPolicyAsync(RadiusPolicyModel policy)
    {
        await GetTokenAsync();
        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }
        string jsonPayload = JsonSerializer.Serialize(policy);
        APIResultModel createOrEditRadiusPolicyResponse = await _httpClient.CallGenericAsync(
            _url + "/api/Policies/SaveOrCreateRadiusPolicy",
            jsonPayload,
            _token.Token,
            HttpMethod.Post
        );
        if (createOrEditRadiusPolicyResponse.Success)
        {
            return createOrEditRadiusPolicyResponse;
        }
        else
        {
            throw new HttpRequestException(
                "Error creating or editing radius policy" + createOrEditRadiusPolicyResponse.Message
            );
        }
    }

    public async Task<APIResultModel> DeleteRadiusPolicyAsync(RadiusPolicyModel policy)
    {
        await GetTokenAsync();
        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }
        string jsonPayload = JsonSerializer.Serialize(policy);
        APIResultModel deleteRadiusPolicyResponse = await _httpClient.CallGenericAsync(
            _url + "/api/Policies/DeleteRadiusPolicy",
            jsonPayload,
            _token.Token,
            HttpMethod.Post
        );
        if (deleteRadiusPolicyResponse.Success)
        {
            return deleteRadiusPolicyResponse;
        }
        else
        {
            throw new HttpRequestException(
                "Error deleting radius policy" + deleteRadiusPolicyResponse.Message
            );
        }
    }

    public async Task<List<AuthenticationEventModel>> GetAuthAuditLogsAsync(
        TimeFrameModel timeFrame
    )
    {
        await GetTokenAsync();
        if (timeFrame == null)
        {
            throw new ArgumentNullException(nameof(timeFrame));
        }
        AuditRequestModel auditRequest = new(timeFrame);
        string payload = JsonSerializer.Serialize(auditRequest);
        APIResultModel getAuthAuditLogsResponse = await _httpClient.CallGenericAsync(
            _url + "/api/Logs/GetAuthAuditLogs",
            payload,
            _token.Token,
            HttpMethod.Post
        );
        if (getAuthAuditLogsResponse is { Success: true, Message: not null })
        {
            List<AuthenticationEventModel>? authenticationLogs = JsonSerializer.Deserialize<
                List<AuthenticationEventModel>
            >(getAuthAuditLogsResponse.Message);
            if (authenticationLogs == null)
            {
                throw new HttpRequestException("Error deserializing auth logs");
            }
            List<AuthenticationEventModel> authenticationEvents = new();
            authenticationEvents.AddRange(authenticationLogs);
            while (authenticationLogs.Count >= auditRequest.MaxNumberOfRecords)
            {
                auditRequest.PageNumber += 1;
                payload = JsonSerializer.Serialize(auditRequest);
                getAuthAuditLogsResponse = await _httpClient.CallGenericAsync(
                    _url + "/api/Logs/GetAuthAuditLogs",
                    payload,
                    _token.Token,
                    HttpMethod.Post
                );
                if (getAuthAuditLogsResponse is { Success: true, Message: not null })
                {
                    authenticationLogs =
                        JsonSerializer.Deserialize<List<AuthenticationEventModel>>(
                            getAuthAuditLogsResponse.Message
                        ) ?? new();
                    authenticationEvents.AddRange(authenticationLogs);
                }
            }
            return authenticationEvents;
        }
        throw new HttpRequestException(
            "Error getting auth logs" + getAuthAuditLogsResponse.Message
        );
    }

    private async Task GetTokenAsync()
    {
        TokenRequestContext authContext = new([_scopes]);
        if (_azureTokenCredential == null)
        {
            throw new ArgumentNullException(nameof(_azureTokenCredential));
        }
        _token = await _azureTokenCredential.GetTokenAsync(authContext, default);
        if (string.IsNullOrWhiteSpace(_token.Token))
        {
            throw new AuthenticationFailedException("Error getting token");
        }
    }
}
