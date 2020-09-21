using ETCModule.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ETCModule.Models
{
    class EtcRequest
    {
        private static readonly string etcPriceRequest = "https://blockscout.com/etc/mainnet/api?module=stats&action=ethprice";

        private static readonly string etcWalletRequest = "https://blockscout.com/etc/mainnet/api?module=account&action=balance&address={address}";

        private static string Send(string request)
        {
            string result;
            using (WebClient webClient = new WebClient())
            {
                result = webClient.DownloadString(request);
            }
            return result;
        }

        public static EtcPrice GetPrice()
        {
            string request = etcPriceRequest;
            try
            {
                return JsonConvert.DeserializeObject<EtcPrice>(Send(request));
            }
            catch
            {
                return new EtcPrice() { Status = "0" };
            }
        }

        public static EtcWalletBalance GetWalletBalance(string address)
        {
            string request = etcWalletRequest.Replace("{address}", address);
            try
            {
                return JsonConvert.DeserializeObject<EtcWalletBalance>(Send(request));
            }
            catch
            {
                return new EtcWalletBalance() { Result = "0" };
            }
        }
    }
}
