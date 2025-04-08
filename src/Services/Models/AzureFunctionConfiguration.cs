namespace Marketplace.SaaS.Accelerator.Services.Models
{
    public class AzureFunctionConfiguration
    {
        public string BaseUrl { get; set; }
        public string Key { get; set; }
        public int TimeoutInSeconds { get; set; } = 30;
        public int MaxRetries { get; set; } = 3;
    }
} 