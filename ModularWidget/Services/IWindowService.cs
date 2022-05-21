using ModularWidget.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModularWidget.Services
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public Int32 X;
        public Int32 Y;
    };

    public interface IWindowService
    {
        void AddWindow(MainWindow window);

        void RemoveAndCloseWindow(MainWindow window);

        bool CursorInsideWindow(Win32Point w32Mouse);

        MainWindow FindWindowByRegion(RegionUC region);
    }
}
