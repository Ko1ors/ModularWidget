using FearGreedIndexModule.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace FearGreedIndexModule.ViewModels
{
    public class FearGreedIndexViewModel : BaseModel
    {
        private readonly int updateInterval = 5; // in minutes
        private readonly string url = "https://api.alternative.me/";
        private readonly string endpoint = "fng/";
        private Timer timer;

        public FearGreedIndex Index { get; set; }

        public int Value => Index?.Data?.FirstOrDefault()?.Value ?? 0;

        public async Task Start()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            await StartUpdate();
        }

        private async Task StartUpdate()
        {
            Index = await GetFearGreedIndexAsync();
            OnPropertyChanged(nameof(Index));
            var timeUntilUpdate = Index?.Data?.FirstOrDefault()?.TimeUntilUpdate ?? -1;
            if (timeUntilUpdate > 0)
                timer.Interval = timeUntilUpdate * 1000;
            else
                timer.Interval = updateInterval * 1000 * 60;

            timer.Start();

            await Task.CompletedTask;
        }

        private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            await StartUpdate();
        }

        private async Task<FearGreedIndex> GetFearGreedIndexAsync()
        {
            var client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(url + endpoint);
            var result = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<FearGreedIndex>(result);
        }

    }
}
