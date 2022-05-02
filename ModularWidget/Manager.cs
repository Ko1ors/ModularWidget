using ModularWidget.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ModularWidget
{
    public delegate void Notify();
    public static class Manager
    {
        public delegate void RegionHandler(string regName);

        public static event RegionHandler RegionRequested;
        public static event RegionHandler RegionCreated;


        public static void RegionRequest(string regName)
        {
            RegionRequested?.Invoke(regName);
        }

        public static void RegionCreate(string regName)
        {
            RegionCreated?.Invoke(regName);
        }
    }
}
