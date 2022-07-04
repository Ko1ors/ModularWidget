using FearGreedIndexModule.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace FearGreedIndexModule.ViewModels
{
    public class FearGreedIndexViewModel : BaseModel
    {
        private const int updateInterval = 5;
        private const string url = "https://api.alternative.me/";
        private const string endpoint = "fng/";

        private readonly ILogger<FearGreedIndexViewModel> _logger;
        
        private Timer timer;

        public FearGreedIndex Index { get; set; }

        public int Value => Index?.Data?.FirstOrDefault()?.Value ?? 0;

        public FearGreedIndexViewModel(ILogger<FearGreedIndexViewModel> logger)
        {
            _logger = logger;
        }

        public async Task Start()
        {
            _logger.LogInformation("Starting FearGreedIndexViewModel.");
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            await StartUpdate();
        }

        private async Task StartUpdate()
        {
            _logger.LogInformation("Starting FearGreedIndex update.");
            Index = await GetFearGreedIndexAsync();
            OnPropertyChanged(nameof(Index));
            var timeUntilUpdate = Index?.Data?.FirstOrDefault()?.TimeUntilUpdate ?? -1;
            if (timeUntilUpdate > 0)
                timer.Interval = timeUntilUpdate * 1000;
            else
                timer.Interval = updateInterval * 1000 * 60;

            _logger.LogInformation($"Next FearGreedIndex update in {timer.Interval / 1000} seconds.");

            timer.Start();

            await Task.CompletedTask;
        }

        private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            await StartUpdate();
        }

        private async Task<FearGreedIndex> GetFearGreedIndexAsync()
        {
            try
            {
                _logger.LogInformation($"Getting FearGreedIndex. Url: {url}{endpoint}");
                var client = new System.Net.Http.HttpClient();
                var response = await client.GetAsync(url + endpoint);
                var result = await response.Content.ReadAsStringAsync();
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
