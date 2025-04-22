using Microsoft.Extensions.Configuration;

namespace GPE
{
    class ApiCaller
    {
        private readonly IConfiguration _configuration;
        public readonly HttpClient _httpClient = new();

        public ApiCaller()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("ApiSettings.json")
                .Build();
        }

        public string GetConfigurationValue(string key)
        {
            string? value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException($"{key} cannot be null or empty.");
            }
            return value;
        }
    }
}
