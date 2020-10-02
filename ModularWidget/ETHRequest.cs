using ModularWidget.Data;
using Newtonsoft.Json;
using System.Net;

namespace ModularWidget
{
    public static class EthRequest
    {
        private static readonly string ethPriceRequest = "https://api.etherscan.io/api?module=stats&action=ethprice&apikey=";

        private static readonly string ethGasRequest = "https://api.etherscan.io/api?module=gastracker&action=gasoracle&apikey=";

        private static readonly string ethBlockRequest = "https://api.etherscan.io/api?module=block&action=getblockreward&blockno={blocknum}&apikey=";

        private static readonly string ethWalletRequest = "https://api.etherscan.io/api?module=account&action=balance&address={address}&tag=latest&apikey=";

        private static string Send(string request)
        {
            string result;
            using (WebClient webClient = new WebClient())
            {
                result = webClient.DownloadString(request);
            }
            return result;
        }

        public static EthPrice GetPrice(string api)
        {
            string request = ethPriceRequest;
            if (api != null)
                request += api;
            try
            {
                return JsonConvert.DeserializeObject<EthPrice>(Send(request));
            }
            catch
            {
                return new EthPrice() { Status = "0" };
            }
        }

        public static EthGasPrice GetGasPrice(string api)
        {
            string request = ethGasRequest;
            if (api != null)
                request += api;
            try
            {
                return JsonConvert.DeserializeObject<EthGasPrice>(Send(request));
            }
            catch
            {
                return new EthGasPrice() { Status = "0" };
            }
        }

        public static BlockReward GetBlockReward(string api, string blocknum)
        {
            string request = ethBlockRequest.Replace("{blocknum}", blocknum);
            if (api != null)
                request += api;
            try
            {
                return JsonConvert.DeserializeObject<BlockReward>(Send(request));
            }
            catch
            {
                return new BlockReward() { Status = "0" };
            }
        }

        public static WalletBalance GetWalletBalance(string api, string address)
        {
            string request = ethWalletRequest.Replace("{address}", address);
            if (api != null)
                request += api;
            try
            {
                return JsonConvert.DeserializeObject<WalletBalance>(Send(request));
            }
            catch
            {
                return new WalletBalance() { Status = "0" };
            }
        }
    }
}
