namespace GPE.Farnell
{
    class ApiCaller : GPE.ApiCaller
    {
        private readonly string _termType = "manuPartNum";
        private readonly string _storeInfoId = "dk.farnell.com";
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.element14.com/catalog/products";

        public ApiCaller() {
            _apiKey = GetConfigurationValue("Farnell:ApiKey");
        }

        public async Task GetProductDetailsAsync(string productNumber)
        {
            string requestUrl = $"{_baseUrl}?callInfo.responseDataFormat=JSON&term={_termType}:{productNumber}&storeInfo.id={_storeInfoId}&callInfo.apiKey={_apiKey}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
