using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.Services.Services
{
    public class AzureFunctionService : IAzureFunctionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureFunctionService> _logger;
        private readonly string _functionBaseUrl;
        private readonly string _functionKey;

        public AzureFunctionService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AzureFunctionService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            _functionBaseUrl = _configuration["AzureFunctions:BaseUrl"];
            _functionKey = _configuration["AzureFunctions:Key"];
            
            if (string.IsNullOrEmpty(_functionBaseUrl) || string.IsNullOrEmpty(_functionKey))
            {
                throw new InvalidOperationException("Azure Function configuration is missing");
            }
        }

        public async Task<T> InvokeFunctionAsync<T>(string functionName, object payload)
        {
            try
            {
                var url = $"{_functionBaseUrl}/api/{functionName}";
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Check if response is empty
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    _logger.LogWarning("Empty response received from Azure Function {FunctionName}", functionName);
                    return default(T);
                }

                // Try to parse as JSON first
                try
                {
                    return JsonSerializer.Deserialize<T>(responseContent);
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, check if it's a plain text response
                    if (typeof(T) == typeof(string))
                    {
                        return (T)(object)responseContent;
                    }
                    
                    _logger.LogWarning("Received plain text response from Azure Function {FunctionName}: {Response}", 
                        functionName, responseContent);
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking Azure Function {FunctionName}", functionName);
                throw;
            }
        }

        public async Task<T> InvokeFunctionWithRetryAsync<T>(string functionName, object payload, int maxRetries = 3)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    return await InvokeFunctionAsync<T>(functionName, payload);
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        _logger.LogError(ex, "Failed to invoke Azure Function {FunctionName} after {RetryCount} attempts", 
                            functionName, retryCount);
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount))); // Exponential backoff
                }
            }
        }

        public async Task<bool> ValidateFunctionResponseAsync(string functionName, object response)
        {
            try
            {
                // Add your validation logic here
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Azure Function response for {FunctionName}", functionName);
                return false;
            }
        }
    }
} 