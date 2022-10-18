using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using ModularWidget.Common.Clients;
using ModularWidget.Models;
using ModularWidget.Services;
using ModularWidget.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace ModularWidget
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private static IContainerProvider _container;

        public App()
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            var serviceCollection = new ServiceCollection();

            Log.Logger = new LoggerConfiguration().WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {SourceContext} {Method} | {Message} {Exception}{NewLine}").CreateLogger();
            serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            var container = new UnityContainer();
            container.BuildServiceProvider(serviceCollection);

            return new UnityContainerExtension(container);
        }

        protected override void InitializeModules()
        {
            Log.Logger.ForContext<App>().Information("Application started", GetType());
            base.InitializeModules();
            InitNotifyIcon();
        }

        private void InitNotifyIcon()
        {
            var rootElement = new NotifyIconElement
            {
                Name = "modules",
                Header = "Loaded modules",
                Children = new ObservableCollection<NotifyIconElement>()
            };
            rootElement.Children.AddRange(Resolve<IModuleManager>().Modules.Select(m => new NotifyIconElement() { Name = m.ModuleName, Header = m.ModuleType.Split('.')[0] }));
            Resolve<INotifyIconService>().AddNotifyIconElement(rootElement);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Logger.ForContext<App>().Fatal(e.ExceptionObject as Exception, "Unhandled Exception: ");
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Logger.ForContext<App>().Fatal(e.Exception, "UI Thread Unhandled Exception: ");
            Current.Shutdown();
            e.Handled = true;
        }

        protected override Window CreateShell()
        {
            return Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<AppSettings>();
            containerRegistry.RegisterSingleton<IRegionService, RegionService>();
            containerRegistry.RegisterSingleton<IWindowService, WindowService>();
            containerRegistry.RegisterSingleton<INotifyIconService, NotifyIconService>();

            containerRegistry.Register<NotifyIconViewModel>();

            containerRegistry.RegisterSingleton<ModularHttpClient>();
            
            _container = Container;

            // Initialize tray notify icon
            var taskbarIcon = (TaskbarIcon)FindResource("TrayNotifyIcon");
            Resolve<INotifyIconService>().SetTaskbarIcon(taskbarIcon);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
        }
    }
}
