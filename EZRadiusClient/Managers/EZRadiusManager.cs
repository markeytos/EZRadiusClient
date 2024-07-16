using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using EZRadiusClient.Services;
using EZRadiusClient.Models;
using Microsoft.Extensions.Logging;

namespace EZRadiusClient.Managers;

public interface IEZRadiusManager
{
    /// <summary>
    /// Gets the current Radius policies from the EZRadius Database
    /// </summary>
    /// <returns>List of <see cref="RadiusPolicyModel"/></returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<List<RadiusPolicyModel>> GetRadiusPolicies();
    
    /// <summary>
    /// Creates a new Radius policy in the EZRadius Database with passed policy
    /// </summary>
    /// <param name="policy"></param> Model containing attributes for policy to be created
    /// <returns>List of <see cref="APIResultModel"/></returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<APIResultModel> CreateRadiusPolicy(RadiusPolicyModel policy);
    
    /// <summary>
    /// Edits an existing Radius policy in the EZRadius Database with passed policy
    /// </summary>
    /// <param name="policy"></param> Model containing attributes for policy to be edited
    /// <returns></returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<APIResultModel> EditRadiusPolicy(RadiusPolicyModel policy);

    /// <summary>
    /// Deletes an existing Radius policy in the EZRadius Database with passed policy
    /// </summary>
    /// <param name="policy"></param> Model containing attributes for policy to be deleted
    /// <returns></returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<APIResultModel> DeleteRadiusPolicy(RadiusPolicyModel policy);
}

public class EZRadiusManager : IEZRadiusManager
{
    private readonly HttpClientService _httpClient;
    private readonly string _url;
    private AccessToken _token;
    private readonly TokenCredential? _azureTokenCredential;

    public EZRadiusManager(HttpClient httpClient, ILogger? logger, TokenCredential? azureTokenCredential, string baseUrl = "https://test.ezradius.io/")
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl));
        }
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
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
    }

    public async Task<List<RadiusPolicyModel>> GetRadiusPolicies()
    {
        await GetTokenAsync();
        APIResultModel getRadiusPoliciesResponse = await _httpClient.CallGenericAsync(_url + "api/Policies/GetRadiusPolicies", null, _token.Token, HttpMethod.Get);
        if (getRadiusPoliciesResponse.Success)
        {
            List<RadiusPolicyModel>? currentRadiusPolicies = JsonSerializer.Deserialize<List<RadiusPolicyModel>>(getRadiusPoliciesResponse.Message);
            if (currentRadiusPolicies == null)
            {
                throw new HttpRequestException("Error deserializing current radius policies");
            }

            return currentRadiusPolicies;
        }
        else
        {
            throw new HttpRequestException("Error getting current radius policies" + getRadiusPoliciesResponse.Message);
        }
    }
    
    public async Task<APIResultModel> CreateRadiusPolicy(RadiusPolicyModel policy)
    {
        await GetTokenAsync();
        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }
        string jsonPayload = JsonSerializer.Serialize(policy);
        APIResultModel createRadiusPolicyResponse = await _httpClient.CallGenericAsync(_url + "api/Policies/SaveOrCreateRadiusPolicy", jsonPayload, _token.Token, HttpMethod.Post);
        if (createRadiusPolicyResponse.Success)
        {
            return createRadiusPolicyResponse;
        }
        else
        {
            throw new HttpRequestException("Error creating radius policy" + createRadiusPolicyResponse.Message);
        }
    }
    
    public async Task<APIResultModel> EditRadiusPolicy(RadiusPolicyModel policy)
    {
        await GetTokenAsync();
        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }
        string jsonPayload = JsonSerializer.Serialize(policy);
        APIResultModel editRadiusPolicyResponse = await _httpClient.CallGenericAsync(_url + "api/Policies/SaveOrCreateRadiusPolicy", jsonPayload, _token.Token, HttpMethod.Put);
        if (editRadiusPolicyResponse.Success)
        {
            return editRadiusPolicyResponse;
        }
        else
        {
            throw new HttpRequestException("Error editing radius policy" + editRadiusPolicyResponse.Message);
        }
    }
    
    public async Task<APIResultModel> DeleteRadiusPolicy(RadiusPolicyModel policy)
    {
        await GetTokenAsync();
        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }
        string jsonPayload = JsonSerializer.Serialize(policy);
        APIResultModel deleteRadiusPolicyResponse = await _httpClient.CallGenericAsync(_url + "api/Policies/DeleteRadiusPolicy", jsonPayload, _token.Token, HttpMethod.Delete);
        if (deleteRadiusPolicyResponse.Success)
        {
            return deleteRadiusPolicyResponse;
        }
        else
        {
            throw new HttpRequestException("Error deleting radius policy" + deleteRadiusPolicyResponse.Message);
        }
    }
    
    private async Task GetTokenAsync()
    {
        TokenRequestContext authContext =
            new(new[] { "https://management.core.windows.net/.default" });
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