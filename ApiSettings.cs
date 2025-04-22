using Microsoft.Extensions.Configuration;

namespace GPE
{
    public class ApiSettings(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
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
