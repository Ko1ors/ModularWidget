using ETCModule.Models;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<EtcClientService> _logger;

        public EtcClientService(ILogger<EtcClientService> logger)
        {
            _logger = logger;
        }

        private async Task<string> SendAsync(string request)
        {
            string result;
            using (var client = new HttpClient())
            {
                result = await client.GetStringAsync(request);
                _logger.LogInformation($"Request: {request}. Response: {result}");
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
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting price: {request}.");
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
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting random wallets: {request}.");
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
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting wallet balance: {request}.");
                return new EtcWalletBalanceResult() { Status = "0" };
            }
        }
    }
}
