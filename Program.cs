using Digikey;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GPE
{
    static class Program
    {
        private static string? _oauthToken;
        private static DateTime _tokenExpiration;

        static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("DigiKey/ApiSettings.json")
                .Build();

            var clientSecret = configuration["ApiSettings:ClientSecret"];
            var clientId = configuration["ApiSettings:ClientId"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                Console.WriteLine("ClientId or ClientSecret is missing in the configuration.");
                return;
            }

            var client = new Client(new HttpClient())
            {
                BaseUrl = configuration["ApiSettings:ProductSearchBaseUrl"]
            };

            while (true)
            {
                Console.WriteLine("Select an endpoint to call:");
                Console.WriteLine("1. Keyword Search");
                Console.WriteLine("2. Product Details");
                Console.WriteLine("3. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await CallKeywordSearchAsync(client, clientId);
                        break;
                    case "2":
                        await CallProductDetailsAsync(client, clientId);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static async Task CallKeywordSearchAsync(Client client, string clientId)
        {
            await EnsureValidTokenAsync();

            var keywordRequest = new KeywordRequest
            {
                Keywords = "resistor",
                Limit = 10,
                Offset = 0
            };

            try
            {
                var response = await client.KeywordSearchAsync(
                    null,
                    $"Bearer {_oauthToken}",
                    clientId,
                    "en",
                    "USD",
                    "US",
                    null,
                    keywordRequest);
                foreach (var product in response.Products)
                {
                    Console.WriteLine($"Product: {product.Description.ProductDescription}, Price: {product.UnitPrice}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task CallProductDetailsAsync(Client client, string clientId)
        {
            await EnsureValidTokenAsync();

            Console.WriteLine("Enter product number:");
            var productNumber = Console.ReadLine();

            try
            {
                var response = await client.ProductDetailsAsync(
                    productNumber,
                    null,
                    null,
                    $"Bearer {_oauthToken}",
                    clientId,
                    "en",
                    "USD",
                    "US",
                    null);
                Console.WriteLine($"Product: {response.Product.Description.ProductDescription}, Price: {response.Product.UnitPrice}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task EnsureValidTokenAsync()
        {
            if (DateTime.UtcNow >= _tokenExpiration)
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("DigiKey/ApiSettings.json")
                    .Build();

                await GetOAuthTokenAsync(configuration);
            }
        }

        private static async Task GetOAuthTokenAsync(IConfiguration configuration)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, configuration["ApiSettings:AccessTokenEndPoint"]);
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", configuration["ApiSettings:ClientId"] ?? ""),
                new KeyValuePair<string, string>("client_secret", configuration["ApiSettings:ClientSecret"] ?? "")
            ]);
            request.Content = content;

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(responseContent);
            _oauthToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);
        }

        private class OAuthTokenResponse
        {
            [JsonProperty("access_token")]
            public required string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}
