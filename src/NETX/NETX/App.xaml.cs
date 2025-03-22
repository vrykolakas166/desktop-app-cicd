using CommunityToolkit.Mvvm.DependencyInjection;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NETX.Core;
using NETX.Extensions;
using NETX.Helpers;
using NETX.Services;
using NETX.Services.Interfaces;
using NETX.ViewModels;
using NETX.Views;
using NETX.Views.Layout;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.MemorySink;
using System.Windows;

namespace NETX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow? _window;
        private MaintainenceWindow? _maintainenceWindow;

        /// <summary> 
        /// Gets the window for the application instance.
        /// </summary>
        public MainWindow? Window => _window;
        public MaintainenceWindow? TheMaintainenceWindow => _maintainenceWindow;

        /// <summary>
        /// Gets the current application instance.
        /// </summary>
        public static new App Current => (App)Application.Current;

        public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new();

        public static ILogSource<LogItem> LogSource { get; set; } = new NullLogSource<LogItem>();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // LOG setup
                ConfigureLogSink();

                Log.Information("Launching application...");

                // Create PaletteHelper to manage theme
                Log.Verbose("Configure theme...");
                var paletteHelper = new PaletteHelper();
                var theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(BaseTheme.Light);
                theme.SetCustomPreset();
                paletteHelper.SetTheme(theme);

                // Config services
                Log.Verbose("Configure services...");
                ConfigureServices();

                Log.Verbose("Initialize dbcontext...");
                var dbContextFactory = Ioc.Default.GetService<IDbContextFactory<NXDbContext>>();
                using var dbContext = dbContextFactory?.CreateDbContext();
                dbContext?.Database.Migrate();

                // Open window
                Log.Verbose("Start main window...");
                _window = new()
                {
                    Title = "Home"
                };
                _maintainenceWindow = new();
                _window.Show();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error occurred during application launch.");
                MessageBox.Show($"An error occurred during application launch: {ex.Message}.", "FATAL ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ConfigureLogSink()
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.ControlledBy(LoggingLevelSwitch)
               .WriteTo.MemorySink(
                   out ILogSource<LogItem> logSource,
                   options =>
                   {
                       options.LogEventConverter = logEvent =>
                       {
                           return new LogItem(logEvent.Timestamp, logEvent.Level, logEvent.MessageTemplate.Text);
                       };
                       options.MaxLogsCount = 100_000;
                       options.OnException = ex =>
                       {
                           // Handle exception
                           System.IO.File.WriteAllText("exception.txt", ex.Message);
                       };
                   })
                   .CreateLogger();
            LogSource = logSource;
        }

        private static void ConfigureServices()
        {
            Ioc.Default.ConfigureServices(new ServiceCollection()
                .AddTransient<ControlBarViewModel>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<SettingPageViewModel>()

                .AddDbContextFactory<NXDbContext>()
                .AddSingleton<ISettingService, SettingService>()

                .BuildServiceProvider());
        }

        public void RenewMaintainenceWindow()
        {
            _maintainenceWindow = new();
        }
    }
}
