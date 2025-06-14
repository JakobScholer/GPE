﻿using Digikey;
using Newtonsoft.Json;

namespace GPE.Digikey
{
    class ApiCaller : GPE.ApiCaller
    {
        private string? _oauthToken;
        private DateTime _tokenExpiration;
        private readonly Client _client;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl = "https://api.digikey.com/products/v4";
        private readonly string _tokenUrl = "https://api.digikey.com/v1/oauth2/token";

        public ApiCaller() {
            _oauthToken = null;
            _tokenExpiration = DateTime.MinValue;
            _client = new Client(_httpClient)
            {
                BaseUrl = _baseUrl
            };
            _clientId = GetConfigurationValue("Digikey:ClientId");
            _clientSecret = GetConfigurationValue("Digikey:ClientSecret");
        }

        public async Task GetProductDetailsAsync(string productNumber)
        {
            await EnsureValidTokenAsync();
            try
            {
                var response = await _client.ProductDetailsAsync(
                    productNumber,
                    null,
                    null,
                    $"Bearer {_oauthToken}",
                    _clientId,
                    "da",
                    "DKK",
                    "DK",
                    null);
                Console.WriteLine($"Product: {response.Product.Description.ProductDescription}, Price: {response.Product.UnitPrice}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task EnsureValidTokenAsync()
        {
            if (DateTime.UtcNow >= _tokenExpiration)
            {
                await GetOAuthTokenAsync();
            }
        }

        private async Task GetOAuthTokenAsync()
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _tokenUrl);
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
            ]);
            request.Content = content;

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(responseContent) ?? throw new Exception("Failed to retrieve OAuth token.");
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
