using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Contracts
{
    public interface IAzureFunctionService
    {
        Task<T> InvokeFunctionAsync<T>(string functionName, object payload);
        Task<T> InvokeFunctionWithRetryAsync<T>(string functionName, object payload, int maxRetries = 3);
        Task<bool> ValidateFunctionResponseAsync(string functionName, object response);
    }
} 