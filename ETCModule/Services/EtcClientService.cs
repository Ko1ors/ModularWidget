using ETCModule.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ETCModule.Services
{
    public class EtcClientService : IEtcClientService
    {
        private const string etcPriceRequest = "https://blockscout.com/etc/mainnet/api?module=stats&action=coinprice";
        private const string etcWalletRequest = "https://blockscout.com/etc/mainnet/api?module=account&action=balance&address={address}";
        private const string etcRandomWalletsRequest = "https://blockscout.com/etc/mainnet/api?module=account&action=listaccounts&page={page}&offset={offset}";

        private async Task<string> SendAsync(string request)
        {
            string result;
            using (var client = new HttpClient())
            {
                result = await client.GetStringAsync(request);
            }
            return result;
        }

        public async Task<EtcPriceResult> GetPriceAsync()
        {
            string request = etcPriceRequest;
            try
            {
                return JsonConvert.DeserializeObject<EtcPriceResult>(await SendAsync(request));
            }
            catch
            {
                return new EtcPriceResult() { Status = "0" };
            }
        }

        public async Task<EtcWalletsResult> GetRandomWalletsAsync(int count)
        {
            string request = etcRandomWalletsRequest.Replace("{page}", new Random().Next(1, 1000).ToString()).Replace("{offset}", count.ToString());
            try
            {
                return JsonConvert.DeserializeObject<EtcWalletsResult>(await SendAsync(request));
            }
            catch
            {
                return new EtcWalletsResult() { Status = "0" };
            }
        }

        public async Task<EtcWalletBalanceResult> GetWalletBalanceAsync(string address)
        {
            string request = etcWalletRequest.Replace("{address}", address);
            try
            {
                return JsonConvert.DeserializeObject<EtcWalletBalanceResult>(await SendAsync(request));
            }
            catch
            {
                return new EtcWalletBalanceResult() { Status = "0" };
            }
        }
    }
}
