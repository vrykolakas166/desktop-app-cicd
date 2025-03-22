using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NETX.Helpers;
using Serilog;
using Serilog.Events;
using System.Collections.ObjectModel;

namespace NETX.ViewModels.Maintainence
{
    public partial class LogsViewModel : ObservableObject
    {
        public event EventHandler? ScrollToLastRequested;

        [ObservableProperty]
        private bool _isAutoScroll = true;

        [ObservableProperty]
        private bool _updateLogEnabled = true;

        [ObservableProperty]
        public LogEventLevel _selectedLogEventLevel;

        public LogEventLevel[] LogEventLevels { get; } = Enum.GetValues<LogEventLevel>();

        public ObservableCollection<LogItem> LogEvents { get; } = [];

        private CancellationTokenSource? LogViewerUpdateCancelllationTokenSource { get; set; }

        public LogsViewModel()
        {
            Log.Verbose("Initialize logs view model.");
            SelectedLogEventLevel = LogEventLevel.Information;
            LogViewerUpdateCancelllationTokenSource = new();
            _ = KeepFetchingLogs(LogViewerUpdateCancelllationTokenSource.Token);
        }

        public async Task UpdateLogViewer()
        {
            Log.Information($"Update Log Viewer: {UpdateLogEnabled}.");
            {
                if (UpdateLogEnabled)
                {
                    LogViewerUpdateCancelllationTokenSource = new();
                    await KeepFetchingLogs(LogViewerUpdateCancelllationTokenSource.Token);
                }
                else
                {
                    LogViewerUpdateCancelllationTokenSource?.Cancel();
                    LogViewerUpdateCancelllationTokenSource?.Dispose();
                    LogViewerUpdateCancelllationTokenSource = null;
                }
            }
        }

        partial void OnUpdateLogEnabledChanged(bool value)
        {
            _ = UpdateLogViewer();
        }

        private async Task KeepFetchingLogs(CancellationToken cancellationToken)
        {
            try
            {
                using var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

                while (await periodicTimer.WaitForNextTickAsync(cancellationToken) is true)
                {
                    int startIndex = LogEvents.Count;
                    var fetchedLogs = await App.LogSource.GetLogs(startIndex, 10_000, cancellationToken);

                    if (fetchedLogs.Any() is false)
                    {
                        continue;
                    }

                    foreach (var log in fetchedLogs)
                    {
                        LogEvents.Add(log);
                    }

                    if (IsAutoScroll is true &&
                        LogEvents.LastOrDefault() is { } logEvent)
                    {
                        ScrollToLastRequested?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("Log viewer update task was cancelled.");
            }
            catch (Exception exception)
            {
                Log.Error(exception, "An error occurred while fetching logs.");
            }
        }

        partial void OnSelectedLogEventLevelChanged(LogEventLevel value)
        {
            SetMinimumLevel();
        }

        public void SetMinimumLevel()
        {
            Log.Information($"Set minimum level: {SelectedLogEventLevel}.");
            App.LoggingLevelSwitch.MinimumLevel = SelectedLogEventLevel;
        }

        [RelayCommand]
        public async Task ClearLogs()
        {
            LogEvents.Clear();
            await App.LogSource.ClearLogs();
        }
    }
}
