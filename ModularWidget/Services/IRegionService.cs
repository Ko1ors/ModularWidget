namespace ModularWidget.Services
{
    public delegate void RegionHandler(string regName);

    public interface IRegionService
    {
        event RegionHandler RegionRequested;
        event RegionHandler RegionCreated;

        void RegionRequest(string regName);

        void RegionCreate(string regName);
    }
}
