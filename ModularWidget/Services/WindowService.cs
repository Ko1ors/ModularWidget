using ModularWidget.UserControls;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ModularWidget.Services
{
    public class WindowService : IWindowService
    {
        private readonly List<MainWindow> _windows;

        public WindowService()
        {
            _windows = new List<MainWindow>();
        }

        public void AddWindow(MainWindow window)
        {
            _windows.Add(window);
        }

        public bool CursorInsideWindow(Win32Point w32Mouse)
        {
            Rect rect;
            foreach (MainWindow window in _windows)
            {
                rect = new Rect(window.Left, window.Top, window.Width, window.Height);
                if (rect.Contains(w32Mouse.X, w32Mouse.Y))
                    return true;
            }
            return false;
        }

        public MainWindow FindWindowByRegion(RegionUC region)
        {
            return _windows.Where(w => w.RegionListView.Items.Contains(region)).FirstOrDefault();
        }

        public void RemoveAndCloseWindow(MainWindow window)
        {
            _windows.Remove(window);
            window.Close();
        }
    }
}
