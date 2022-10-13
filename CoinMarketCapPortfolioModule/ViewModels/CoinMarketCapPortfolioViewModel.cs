using CoinMarketCapPortfolioModule.Models;
using CoinMarketCapPortfolioModule.Models.API;
using Microsoft.Extensions.Logging;
using ModularWidget;
using ModularWidget.Commands;
using ModularWidget.Common.ViewModels;
using ModularWidget.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CoinMarketCapPortfolioModule.ViewModels
{
    public class CoinMarketCapPortfolioViewModel : ViewModelBase
    {
        // TODO: Move UpdateTime settings
        private const int UpdateTime = 5; // in minutes
        private const string PortfolioGroupUrl = "https://api.coinmarketcap.com/asset/v3/portfolio/group/queryAll";
        private const string PortfolioDetailsUrl = "https://api.coinmarketcap.com/asset/v3/portfolio/query";
        private const string PortfolioStatisticsUrl = "https://api.coinmarketcap.com/asset/v3/portfolio/queryStatistics";
        private const string filePath = "CoinMarketCapPortfolio.json";
        private const string fileTempPath = "CoinMarketCapPortfolioTemp.json";

        private readonly SettingsMenu _settingsMenu;
        private readonly ILogger<CoinMarketCapPortfolioViewModel> _logger;

        private Timer _timer;
        private HttpClient _httpClient;
        private PortfolioModel _portfolio;

        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                    _httpClient = new HttpClient();
                return _httpClient;
            }
        }

        public PortfolioModel Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                OnPropertyChanged("Portfolio");
            }
        }

        public RelayCommand PrivacyModeToggleCommand { get; private set; }

        public CoinMarketCapPortfolioViewModel(AppSettings settings, ILogger<CoinMarketCapPortfolioViewModel> logger)
        {
            _settingsMenu = settings.GetMenu(Constants.Menu.MenuKey);
            _logger = logger;
            Portfolio = new PortfolioModel() { Name = "Portfolio" };
            PrivacyModeToggleCommand = new RelayCommand((obj) => PrivacyModeToggle());
        }

        public void PrivacyModeToggle()
        {
            Portfolio.PrivacyMode = !Portfolio.PrivacyMode;
            SavePortfolio();
        }

        public async Task StartAsync()
        {
            _logger.LogInformation("Starting CoinMarketCapPortfolioViewModel");
            LoadPortfolio();
            SetTimer();
            await UpdatePortfolioAsync();
        }

        private void SetTimer()
        {
            _timer = new Timer(UpdateTime * 60 * 1000);
            _timer.Elapsed += (s, e) => { _ = UpdatePortfolioAsync(); };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void LoadPortfolio()
        {
            try
            {
                _logger.LogInformation("Loading portfolio from file");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Portfolio file not found");
                    return;
                }

                Portfolio = JsonConvert.DeserializeObject<PortfolioModel>(File.ReadAllText(filePath));
                _logger.LogInformation("Portfolio loaded from file");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading portfolio");
            }
        }

        private void SavePortfolio()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.WriteAllText(fileTempPath, JsonConvert.SerializeObject(Portfolio));
                    File.Replace(fileTempPath, filePath, null);
                }
                else
                {
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(Portfolio));
                }
                _logger.LogInformation("Portfolio saved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving portfolio");
            }
        }

        public async Task UpdatePortfolioAsync()
        {
            try
            {
                // get bearer token from settings
                var bearerToken = _settingsMenu.Get<string>(Constants.Parameters.AuthTokern);
                if (string.IsNullOrEmpty(bearerToken))
                {
                    _logger.LogWarning("Bearer token is empty");
                    return;
                }

                // get group portfolio
                var groupPortfolio = await GetGroupPortfolioAsync(bearerToken);
                if (groupPortfolio == null)
                {
                    _logger.LogWarning("Group portfolio result is null");
                    return;
                }
                if (groupPortfolio.Status.ErrorCode != "0")
                {
                    _logger.LogWarning($"Group portfolio error: {groupPortfolio.Status.ErrorMessage}. Response model: {JsonConvert.SerializeObject(groupPortfolio)}");
                    return;
                }
                if (groupPortfolio.Data == null || !groupPortfolio.Data.Any())
                {
                    _logger.LogWarning($"Group portfolios are empty or null. Response model: {JsonConvert.SerializeObject(groupPortfolio)}");
                    return;
                }

                // find main portfolio
                var mainPortfolio = groupPortfolio.Data.FirstOrDefault(p => p.IsMain);
                if (mainPortfolio == null)
                {
                    _logger.LogWarning($"Main portfolio is null. Response model: {JsonConvert.SerializeObject(groupPortfolio)}");
                    return;
                }

                var resultPortfolio = new PortfolioModel()
                {
                    Name = mainPortfolio.PortfolioName,
                    Price = (decimal)mainPortfolio.TotalAmount,
                    PrivacyMode = Portfolio?.PrivacyMode ?? false,
                };

                // get main portfolio details
                //var mainPortfolioDetails = await GetPortfolioDetailsAsync(bearerToken, mainPortfolio.PortfolioSourceId);
                //if(mainPortfolioDetails == null)
                //{
                //    _logger.LogWarning($"Main portfolio details result is null");
                //    return;
                //}
                //if(mainPortfolioDetails.Status.ErrorCode != "0")
                //{
                //    _logger.LogWarning($"Main portfolio details error: {mainPortfolioDetails.Status.ErrorMessage}. Response model: {JsonConvert.SerializeObject(mainPortfolioDetails)}");
                //    return;
                //}
                //if(mainPortfolioDetails.Data == null || !groupPortfolio.Data.Any())
                //{
                //    _logger.LogWarning($"Main portfolio details is empty or null. Response model: {JsonConvert.SerializeObject(mainPortfolioDetails)}");
                //    return;
                //}
                //mainPortfolio = mainPortfolioDetails.Data.FirstOrDefault();

                // get main portfolio statistics
                var mainPortfolioStatistics = await GetPortfolioStatisticsAsync(bearerToken, mainPortfolio.PortfolioSourceId);
                if (mainPortfolioStatistics == null)
                {
                    _logger.LogWarning($"Main portfolio statistics result is null");
                    return;
                }
                if (mainPortfolioStatistics.Status.ErrorCode != "0")
                {
                    _logger.LogWarning($"Main portfolio statistics error: {mainPortfolioStatistics.Status.ErrorMessage}. Response model: {JsonConvert.SerializeObject(mainPortfolioStatistics)}");
                    return;
                }
                if (mainPortfolioStatistics.Data == null)
                {
                    _logger.LogWarning($"Main portfolio statistics is empty or null. Response model: {JsonConvert.SerializeObject(mainPortfolioStatistics)}");
                    return;
                }

                resultPortfolio.ChangePercent = (decimal)(mainPortfolioStatistics.Data.YesterdayBalancePercent ?? 0);
                resultPortfolio.ChangeBalance = (decimal)mainPortfolioStatistics.Data.YesterdayChangeBalance;
                Portfolio = resultPortfolio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating portfolio");
            }

            SavePortfolio();
        }


        // TODO: move to services
        public async Task<PortfolioResponse<List<Portfolio>>> GetGroupPortfolioAsync(string bearerToken)
        {
            return await PostAsync<PortfolioResponse<List<Portfolio>>>(bearerToken, PortfolioGroupUrl, new { });
        }

        public async Task<PortfolioResponse<List<Portfolio>>> GetPortfolioDetailsAsync(string bearerToken, string portfolioId)
        {
            return await PostAsync<PortfolioResponse<List<Portfolio>>>(bearerToken, PortfolioDetailsUrl, new PortfolioRequest() { PortfolioSourceId = portfolioId, CryptoUnit = 2781, CurrentPage = 1, PageSize = 1000 });
        }

        public async Task<PortfolioResponse<PortfolioStatistics>> GetPortfolioStatisticsAsync(string bearerToken, string portfolioId)
        {
            return await PostAsync<PortfolioResponse<PortfolioStatistics>>(bearerToken, PortfolioStatisticsUrl, new PortfolioRequest() { PortfolioSourceId = portfolioId, CryptoUnit = 2781, FiatUnit = 2781 });
        }

        public async Task<T> PostAsync<T>(string bearerToken, string url, object body = null)
        {
            HttpClient.DefaultRequestHeaders.Remove("Authorization");
            HttpClient.DefaultRequestHeaders.Add("Authorization", bearerToken.StartsWith("Bearer") ? bearerToken : $"Bearer {bearerToken}");
            if (string.IsNullOrEmpty(HttpClient.DefaultRequestHeaders.UserAgent.ToString()))
                HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<T>(responseString);
            return responseObject;
        }
    }
}
