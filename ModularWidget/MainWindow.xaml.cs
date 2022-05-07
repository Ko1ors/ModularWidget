using CommonServiceLocator;
using ModularWidget.UserControls;
using Prism.Regions;
using System;
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
        private readonly AppSettings _appSettings;

        private Window _dragdropWindow = null;
        private Effect _regionEffect = null;

        public MainWindow(AppSettings settings)
        {

            InitializeComponent();
            _appSettings = settings;
            IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            RegionManager.SetRegionManager(regionListView, regionManager);

            Left = SystemParameters.PrimaryScreenWidth - Width * 1.1;
            Top = SystemParameters.PrimaryScreenHeight * 0.05;

            Manager.RegionRequested += CreateRegion;
            _appSettings.Load();

            Style itemContainerStyle = regionListView.ItemContainerStyle;
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

                    DragDrop.AddQueryContinueDragHandler(draggedItem, DragContrinueHandler);
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }

        }

        private void DragContrinueHandler(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Continue && e.KeyStates != DragDropKeyStates.LeftMouseButton)
            {
                DragDropReset((e.OriginalSource as ListViewItem).Content as RegionUC);
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

            int removedIdx = regionListView.Items.IndexOf(droppedRegion);
            int targetIdx = regionListView.Items.IndexOf(target);
            if (removedIdx != targetIdx)
            {
                regionListView.Items.RemoveAt(removedIdx);
                regionListView.Items.Insert(targetIdx, droppedRegion);
            }
            DragDropReset(droppedRegion);
        }

        private void RegionListView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

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

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings(_appSettings);
            settingsWindow.ShowDialog();
        }

        private void CreateRegion(string regName)
        {
            RegionUC reg = new RegionUC();
            reg.RegionName = regName;
            AddRegion(reg);
            Manager.RegionCreate(reg.RegionName);
        }

        private void AddRegion(RegionUC reg)
        {
            regionListView.Items.Add(reg);
        }
    }
}
