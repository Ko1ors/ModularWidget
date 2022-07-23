namespace ETHModule.Data
{
    public class ETHCompositeModel
    {
        public EthPrice EthPrice { get; set; }

        public EthGasPrice EthGasPrice { get; set; }

        public double AvgBlockReward { get; set; }

        public double WalletBalance { get; set; }
    }
}
