using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModularWidget.Common.Clients
{
    public class ModularHttpClient
    {
        private readonly ILogger<ModularHttpClient> _logger;
        
        private HttpClient? _httpClient;

        public HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                    _httpClient = new HttpClient();
                return _httpClient;
            }
        }

        public ModularHttpClient(ILogger<ModularHttpClient> logger)
        {
            _logger = logger;
        }
        
        public async Task<string> GetAsync(string requestUrl)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(requestUrl);
            string responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"ModularHttpClient Get. Request: {requestUrl}. Response: {responseContent.Replace(System.Environment.NewLine, "").Replace("\n", "").Replace("\r", "")}");
            return responseContent;
        }
    }
}
