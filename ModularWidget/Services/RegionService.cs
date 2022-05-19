namespace ModularWidget.Services
{
    public class RegionService : IRegionService
    {
        public event RegionHandler RegionRequested;
        public event RegionHandler RegionCreated;

        public void RegionRequest(string regName)
        {
            RegionRequested?.Invoke(regName);
        }

        public void RegionCreate(string regName)
        {
            RegionCreated?.Invoke(regName);
        }
    }
}
