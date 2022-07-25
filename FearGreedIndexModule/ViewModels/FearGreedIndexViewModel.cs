using FearGreedIndexModule.Models;
using Microsoft.Extensions.Logging;
using ModularWidget.Common.Clients;
using ModularWidget.Common.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace FearGreedIndexModule.ViewModels
{
    public class FearGreedIndexViewModel : ViewModelBase
    {
        private const int updateInterval = 5;
        private const string url = "https://api.alternative.me/";
        private const string endpoint = "fng/";

        private readonly ILogger<FearGreedIndexViewModel> _logger;
        private readonly Timer _timer;
        private readonly ModularHttpClient _httpClient;

        public FearGreedIndex? Index { get; set; }

        public int Value => Index?.Data?.FirstOrDefault()?.Value ?? 0;

        public FearGreedIndexViewModel(ModularHttpClient httpClient, ILogger<FearGreedIndexViewModel> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _timer = new Timer();
        }

        public async Task StartAsync()
        {
            _logger.LogInformation("Starting FearGreedIndexViewModel.");

            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = false;
            await StartUpdate();
        }

        private async Task StartUpdate()
        {
            _logger.LogInformation("Starting FearGreedIndex update.");
            Index = await GetFearGreedIndexAsync();
            OnPropertyChanged(nameof(Index));
            var timeUntilUpdate = Index?.Data?.FirstOrDefault()?.TimeUntilUpdate ?? -1;
            if (timeUntilUpdate > 0)
                _timer.Interval = timeUntilUpdate * 1000;
            else
                _timer.Interval = updateInterval * 1000 * 60;

            _logger.LogInformation($"Next FearGreedIndex update in {_timer.Interval / 1000} seconds.");

            _timer.Start();

            await Task.CompletedTask;
        }

        private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            await StartUpdate();
        }

        private async Task<FearGreedIndex?> GetFearGreedIndexAsync()
        {
            try
            {
                _logger.LogInformation($"Getting FearGreedIndex. Url: {url}{endpoint}");
                var client = new System.Net.Http.HttpClient();
                var result = await _httpClient.GetAsync(url + endpoint);
                _logger.LogInformation($"FearGreedIndex response: {result.Replace("\n", "")}");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<FearGreedIndex>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting FearGreedIndex.");
                return null;
            }
        }

    }
}
