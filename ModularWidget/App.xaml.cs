using Microsoft.Extensions.DependencyInjection;
using ModularWidget.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Prism.Unity.Ioc;
using Serilog;
using System;
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

        public App()
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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

        public override void Initialize()
        {
            Log.Logger.ForContext<App>().Information("Application started", GetType());
            base.Initialize();
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
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<AppSettings>();
            containerRegistry.RegisterSingleton<IRegionService, RegionService>();
            containerRegistry.RegisterSingleton<IWindowService, WindowService>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
        }
    }
}
