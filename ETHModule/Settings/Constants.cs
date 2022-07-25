namespace ETHModule.Settings
{
    public class Constants
    {
        public static class Menu
        {
            public const string MenuKey = "ethModuleSettings";
        }

        public static class Parameters
        {
            public const string ApiKey = "ethApiKey";
            public const string Wallet = "ethWallet";
            public const string UpdateTime = "ethUpdateTime";
            public const string hideBlockReward = "HideEthBlockReward";
            public const string hidePrice = "HideEthPrice";
            public const string hideGasTracker = "HideEthGasTracker";
        }

        public static class Endpoints
        {
            public const string EthBase = "https://api.etherscan.io/api";

            public const string EthPriceRequest = $"{EthBase}?module=stats&action=ethprice&apikey={{apikey}}";

            public const string EthGasRequest = $"{EthBase}?module=gastracker&action=gasoracle&apikey={{apikey}}";

            public const string EthBlockRequest = $"{EthBase}?module=block&action=getblockreward&blockno={{blocknum}}&apikey={{apikey}}";

            public const string EthWalletRequest = $"{EthBase}?module=account&action=balance&address={{address}}&tag=latest&apikey={{apikey}}";
        }
    }
}
