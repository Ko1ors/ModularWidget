using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CryptoMarketCapModule.Services
{
    public static class Request
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> SendAsync(string request)
        {
            using (WebClient webClient = new WebClient())
            {
                return await webClient.DownloadStringTaskAsync(request);
            }
        }
    }
}
