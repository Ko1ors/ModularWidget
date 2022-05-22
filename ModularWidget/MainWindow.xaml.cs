using ModularWidget.Services;
using ModularWidget.UserControls;
using Prism.Regions;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;


namespace ModularWidget
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppSettings _appSettings;
        private IRegionManager _regionManager;
        private IRegionService _regionService;
        private IWindowService _windowService;

        private Window _dragdropWindow = null;
        private Effect _regionEffect = null;


        public MainWindow(AppSettings settings, IRegionManager regionManager, IRegionService regionService, IWindowService windowService, RegionUC region)
        {
            Init(settings, regionManager, regionService, windowService);
            if (!string.IsNullOrEmpty(region.RegionName))
                AddRegion(region);
        }

        public void Init(AppSettings settings, IRegionManager regionManager, IRegionService regionService, IWindowService windowService)
        {
            InitializeComponent();
            _appSettings = settings;
            _regionManager = regionManager;
            _regionService = regionService;
            _windowService = windowService;

            _windowService.AddWindow(this);

            if (!_appSettings.IsLoaded)
            {
                _appSettings.Load();
                RegionManager.SetRegionManager(RegionListView, _regionManager);
                _regionService.RegionRequested += CreateRegion;
            }

            Left = SystemParameters.PrimaryScreenWidth - Width * 1.1;
            Top = SystemParameters.PrimaryScreenHeight * 0.05;

            Style itemContainerStyle = RegionListView.ItemContainerStyle;
            itemContainerStyle.Setters.Add(new Setter(AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(MouseMoveEvent, new MouseEventHandler(RegionListView_MouseMoveEvent)));
            itemContainerStyle.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(RegionListView_Drop)));
            itemContainerStyle.Setters.Add(new EventSetter(GiveFeedbackEvent, new GiveFeedbackEventHandler(RegionListView_GiveFeedback)));
        }


        private void RegionListView_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is ListViewItem)
                {
                    ListBoxItem draggedItem = sender as ListBoxItem;
                    var region = draggedItem.DataContext as RegionUC;

                    _regionEffect = region.Effect;
                    region.Effect = new DropShadowEffect
                    {
                        Direction = 120,
                        ShadowDepth = 0,
                        Opacity = .75,
                    };
                    CreateDragDropWindow(region);

                    DragDrop.AddQueryContinueDragHandler(draggedItem, DragContinueHandler);
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }

        }

        private void DragContinueHandler(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Continue && e.KeyStates != DragDropKeyStates.LeftMouseButton)
            {
                if (!((e.OriginalSource as ListViewItem).Content is RegionUC region))
                    return;
                Win32Point w32Mouse = new Win32Point();
                GetCursorPos(ref w32Mouse);
                if (!_windowService.CursorInsideWindow(w32Mouse))
                {
                    RegionListView.Items.Remove(region);
                    if (RegionListView.Items.IsEmpty)
                        _windowService.RemoveAndCloseWindow(this);
                    var window = new MainWindow(_appSettings, _regionManager, _regionService, _windowService, region)
                    {
                        Left = w32Mouse.X,
                        Top = w32Mouse.Y
                    };
                    window.Show();
                }
                DragDropReset(region);
            }
        }

        private void DragDropReset(RegionUC droppedRegion)
        {
            droppedRegion.Effect = _regionEffect;
            if (_dragdropWindow != null)
            {
                _dragdropWindow.Close();
                _dragdropWindow = null;
            }
        }

        private void RegionListView_Drop(object sender, DragEventArgs e)
        {
            var droppedRegion = e.Data.GetData(typeof(RegionUC)) as RegionUC;
            var target = ((ListViewItem)(sender)).DataContext as RegionUC;
            MainWindow window = this;
            var removeFrom = RegionListView.Items;
            var insertTo = RegionListView.Items;

            int removedIdx = RegionListView.Items.IndexOf(droppedRegion);
            if (removedIdx == -1)
            {
                window = _windowService.FindWindowByRegion(droppedRegion);
                removeFrom = window.RegionListView.Items;
                removedIdx = removeFrom.IndexOf(droppedRegion);
            }

            int targetIdx = RegionListView.Items.IndexOf(target);
            if (targetIdx == -1)
            {
                insertTo = _windowService.FindWindowByRegion(target).RegionListView.Items;
                targetIdx = removeFrom.IndexOf(target);
            }

            if ((removeFrom != insertTo) || (removedIdx != targetIdx))
            {
                removeFrom.RemoveAt(removedIdx);
                insertTo.Insert(targetIdx, droppedRegion);
            }
            if (removeFrom.IsEmpty)
                _windowService.RemoveAndCloseWindow(window);

            DragDropReset(droppedRegion);
        }

        private void RegionListView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            if (e.Effects != DragDropEffects.Move)
            {
                if (!_windowService.CursorInsideWindow(w32Mouse))
                {
                    e.UseDefaultCursors = false;
                    Mouse.SetCursor(Cursors.Cross);
                    e.Handled = true;
                }
            }
            if (_dragdropWindow != null)
            {
                _dragdropWindow.Left = w32Mouse.X;
                _dragdropWindow.Top = w32Mouse.Y;
            }
        }

        private void CreateDragDropWindow(Visual dragElement)
        {
            _dragdropWindow = new Window();
            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;

            Rectangle r = new Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            _dragdropWindow.Content = r;

            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            _dragdropWindow.Left = w32Mouse.X;
            _dragdropWindow.Top = w32Mouse.Y;
            _dragdropWindow.Show();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _windowService.RemoveAndCloseWindow(this);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_appSettings);
            settingsWindow.ShowDialog();
        }

        private void CreateRegion(string regName)
        {
            RegionUC reg = new RegionUC();
            reg.RegionName = regName;
            AddRegion(reg);
            _regionService.RegionCreate(reg.RegionName);
        }

        private void AddRegion(RegionUC reg)
        {
            RegionListView.Items.Add(reg);
        }
    }
}
