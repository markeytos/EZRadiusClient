using System.Net;
using System.Text;
using EZRadiusClient.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace EZRadiusClient.Services;

public interface IHttpClientService
{
    Task<APIResultModel> CallGenericAsync(
        string url,
        string? jsonPayload,
        string? token,
        HttpMethod httpMethod
    );
}

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly ILogger? _logger;

    public HttpClientService(HttpClient httpClient, ILogger? logger)
    {
        _logger = logger;
        _httpClient = httpClient;
        HttpStatusCode[] httpStatusCodesWorthRetrying =
        {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504
        };
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrInner<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
            .WaitAndRetryAsync(
                new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8) }
            );
    }

    public async Task<APIResultModel> CallGenericAsync(
        string url,
        string? jsonPayload,
        string? token,
        HttpMethod httpMethod
    )
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentNullException(nameof(url));
        }
        if (httpMethod == null)
        {
            throw new ArgumentNullException(nameof(httpMethod));
        }
        APIResultModel apiResult = new();
        try
        {
            HttpResponseMessage responseMessage = await _retryPolicy.ExecuteAsync(
                async () => await CreateAndSendAsync(url, jsonPayload, token, httpMethod)
            );
            apiResult.Message = await responseMessage.Content.ReadAsStringAsync();
            apiResult.Success = responseMessage.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            if (_logger != null)
            {
                _logger.LogError(ex, "Error contacting EZRADIUS");
            }
            apiResult.Success = false;
            if (ex.Message.Contains("One or more errors") && ex.InnerException != null)
            {
                apiResult.Message = ex.InnerException.Message;
            }
            else
            {
                apiResult.Message = ex.Message;
            }
        }
        return apiResult;
    }

    private async Task<HttpResponseMessage> CreateAndSendAsync(
        string url,
        string? jsonPayload,
        string? token,
        HttpMethod method
    )
    {
        HttpRequestMessage requestMessage = new(method, url);
        if (!string.IsNullOrWhiteSpace(jsonPayload))
        {
            requestMessage.Content = new StringContent(
                jsonPayload,
                Encoding.UTF8,
                "application/json"
            );
        }
        if (!string.IsNullOrWhiteSpace(token))
        {
            requestMessage.Headers.Add("Authorization", "Bearer " + token);
        }
        return await _httpClient.SendAsync(requestMessage);
    }
}
