using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NETX.Common;
using NETX.Helpers;
using NETX.Services.Interfaces;
using Serilog;
using System.IO;
using System.Reflection;
using System.Windows;

namespace NETX.ViewModels
{
    public partial class SettingPageViewModel : ObservableObject
    {
        private readonly ISettingService _settingService;

        /// <summary>
        /// Get the current version of the application file
        /// </summary>
        [ObservableProperty]
        private string _version = string.Empty;

        /// <summary>
        /// Get the last update information of the application file
        /// </summary>
        [ObservableProperty]
        private string _lastUpdate = string.Empty;

        /// <summary>
        /// Check version info from server to verify new update
        /// </summary>
        [ObservableProperty]
        private bool _needUpdate;

        /// <summary>
        /// Version file from server that contain fields in VersionFile model
        /// </summary>
        [ObservableProperty]
        private string _versionFileServerUrl = string.Empty;

        /// <summary>
        /// Server folder that contains the application
        /// </summary>
        [ObservableProperty]
        private string _releaseFolderServerUrl = string.Empty;

        [ObservableProperty]
        private ApplicationUpdateInfo _applicationUpdateInfo = new();

        /// <summary>
        /// Preent interupting checking or downloading progress
        /// </summary>
        private static ApplicationUpdateInfo _staticApplicationUpdateInfo = new();

        public SettingPageViewModel(ISettingService settingService)
        {
            Log.Information("Initialize setting page view model.");
            _settingService = settingService;

            _version = string.Empty;
            _lastUpdate = string.Empty;
            _versionFileServerUrl = string.Empty;
            _releaseFolderServerUrl = string.Empty;

            _needUpdate = false;

            GetAppInfo();

            //todo: handle when finish checking
            //todo: handle when finish downloading
            if (!_staticApplicationUpdateInfo.IsChecking && !_staticApplicationUpdateInfo.IsDownloading)
            {
                _applicationUpdateInfo = new ApplicationUpdateInfo
                {
                    UpdateMessage = $"Version {Version}",
                    LastUpdateMessage = $"Last update at {LastUpdate}"
                };
            }
            else
            {
                _applicationUpdateInfo = _staticApplicationUpdateInfo;
            }
        }

        /// <summary>
        /// Get the application information and settings
        /// </summary>
        private void GetAppInfo()
        {
            Log.Information("Retrieve application information.");
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            string exePath = Assembly.GetExecutingAssembly().Location;
            var appFile = new FileInfo(exePath);
            Log.Verbose($"Read on application file path: {appFile.FullName}.");

            Version = $"{version?.Major}.{version?.Minor}.{version?.Build}.{version?.Revision}";
            Log.Verbose($"Application version found: {Version}.");

            LastUpdate = appFile.CreationTime.ToString("HH:MM dd/MM/yyyy");
            Log.Verbose($"Application last update found: {LastUpdate}.");

            InitDefaultSetting();
        }

        /// <summary>
        /// Default settings if first init
        /// </summary>
        private void InitDefaultSetting()
        {
            if (!_settingService.GetAll().Any())
            {
                Log.Verbose("Init default settings.");
                RestoreDefaultVersionFileServerUrl();
                RestoreDefaultReleaseFolderServerUrl();
            }
            else
            {
                VersionFileServerUrl = _settingService.GetByKey(Constants.VERSION_FILE_URL)?.Value ?? string.Empty;
                Log.Verbose($"Set VersionFileServerUrl: {VersionFileServerUrl}.");

                if (string.IsNullOrEmpty(VersionFileServerUrl)) RestoreDefaultVersionFileServerUrl();

                ReleaseFolderServerUrl = _settingService.GetByKey(Constants.RELEASE_FOLDER_URL)?.Value ?? string.Empty;
                Log.Verbose($"Set ReleaseFolderServerUrl: {ReleaseFolderServerUrl}.");

                if (string.IsNullOrEmpty(ReleaseFolderServerUrl)) RestoreDefaultReleaseFolderServerUrl();
            }
        }

        partial void OnVersionFileServerUrlChanging(string? oldValue, string newValue)
        {
            Log.Information("Execute SetVersionFileServerUrl command.");
            if (VersionHelper.IsValidUrl(newValue))
            {
                Log.Verbose($"Valid url: {newValue}.");
                Log.Information("Save VersionFileServerUrl value to config.ini file.");
                _settingService.Update(Constants.VERSION_FILE_URL, newValue);
            }
            else
            {
                throw new Exception("Invalid URL.");
            }
        }

        partial void OnReleaseFolderServerUrlChanging(string? oldValue, string newValue)
        {
            Log.Information("Execute SetReleaseFolderServerUrl command.");
            if (VersionHelper.IsValidUrl(newValue))
            {
                Log.Verbose($"Valid url: {newValue}.");
                Log.Information("Save ReleaseFolderServerUrl value to config.ini file.");
                _settingService.Update(Constants.RELEASE_FOLDER_URL, newValue);
            }
            else
            {
                throw new Exception("Invalid URL.");
            }
        }

        [RelayCommand]
        private async Task CheckUpdateFromServer()
        {
            try
            {
                Log.Information("Check application updates.");

                ApplicationUpdateInfo.CheckUpdateButtonMessage = "Checking...";
                ApplicationUpdateInfo.IsChecking = true;
                ApplicationUpdateInfo.CheckUpdateButtonEnable = false;
                _staticApplicationUpdateInfo = ApplicationUpdateInfo;
#if DEBUG
                await Task.Delay(5000);
                NeedUpdate = true;
#else
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(new Uri(VersionFileServerUrl));
                Log.Verbose($"Fetch: {VersionFileServerUrl}.");

                response.EnsureSuccessStatusCode(); // Throws if not 200-299 status code

                var data = await response.Content.ReadAsStringAsync();
                Log.Verbose($"Read response content.");

                var versionFile = JsonConvert.DeserializeObject<VersionFile>(data);
                Log.Verbose($"JsonConvert to deserialize data to VersionFile type.");

                NeedUpdate = VersionHelper.IsNewVersion(Version, versionFile?.Version);
                Log.Verbose($"Compared version (old - new): {Version} - {versionFile?.Version}.");
#endif


                Log.Verbose($"Assign NeedUpdate to {NeedUpdate}.");

                Log.Verbose($"Set controls based on NeedUpdate: {NeedUpdate}.");
                if (NeedUpdate)
                {
                    ApplicationUpdateInfo.CheckUpdateButtonMessage = string.Empty;

                    ApplicationUpdateInfo.NeedUpdateIconVisible = true;
                    ApplicationUpdateInfo.LatestIconIconVisible = false;

                    ApplicationUpdateInfo.CheckUpdateButtonVisible = false;
                    ApplicationUpdateInfo.UpdateNowButtonVisible = true;

                    ApplicationUpdateInfo.UpdateNowButtonTextVisible = true;
                    ApplicationUpdateInfo.UpdateNowButtonProgressVisible = false;

                    // TODO: installer check
                }
                else
                {
                    ApplicationUpdateInfo.LatestIconIconVisible = true;
                    ApplicationUpdateInfo.NeedUpdateIconVisible = false;

                    ApplicationUpdateInfo.CheckUpdateButtonVisible = true;
                    ApplicationUpdateInfo.UpdateNowButtonVisible = false;

                    ApplicationUpdateInfo.CheckUpdateButtonMessage = "You are up to date.";
                    TaskHelper.RunAfter(TimeSpan.FromSeconds(3), () =>
                    {
                        _staticApplicationUpdateInfo.CheckUpdateButtonMessage
                        = ApplicationUpdateInfo.CheckUpdateButtonMessage = string.Empty;
                    });
                }

                ApplicationUpdateInfo.IsChecking = false;
                ApplicationUpdateInfo.CheckUpdateButtonEnable = true;
                _staticApplicationUpdateInfo = ApplicationUpdateInfo;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception in function CheckUpdateFromServer");
                NeedUpdate = false;
                ApplicationUpdateInfo.IsChecking = false;
                ApplicationUpdateInfo.CheckUpdateButtonEnable = true;
                _staticApplicationUpdateInfo = ApplicationUpdateInfo;
            }
        }

        [RelayCommand]
        private async Task DownloadUpdateFromServer()
        {
            try
            {
                Log.Information("Update application.");

                ApplicationUpdateInfo.CheckUpdateButtonMessage = "Updating...";
                ApplicationUpdateInfo.IsDownloading = true;
                ApplicationUpdateInfo.UpdateNowButtonEnable = false;
                ApplicationUpdateInfo.UpdateNowButtonProgressVisible = true;
                ApplicationUpdateInfo.UpdateNowButtonTextVisible = false;
                _staticApplicationUpdateInfo = ApplicationUpdateInfo;
#if DEBUG
                // fake updating 
                for (int i = 1; i <= 1000; i++)
                {
                    await Task.Delay(100);
                    Log.Verbose($"Download {i}/1000");
                }
                Log.Verbose($"Copy to app location: {AppDomain.CurrentDomain.BaseDirectory}");
#else
                // todo
#endif

                ApplicationUpdateInfo.IsDownloading = false;
                ApplicationUpdateInfo.LatestIconIconVisible = true;
                ApplicationUpdateInfo.NeedUpdateIconVisible = false;
                ApplicationUpdateInfo.CheckUpdateButtonVisible = true;

                ApplicationUpdateInfo.UpdateNowButtonVisible = false;
                ApplicationUpdateInfo.UpdateNowButtonEnable = true;
                ApplicationUpdateInfo.UpdateNowButtonProgressVisible = false;
                ApplicationUpdateInfo.UpdateNowButtonTextVisible = true;
                ApplicationUpdateInfo.CheckUpdateButtonEnable = false;

                ApplicationUpdateInfo.CheckUpdateButtonMessage = "Update finished.";
                _staticApplicationUpdateInfo = ApplicationUpdateInfo;

                TaskHelper.RunAfter(TimeSpan.FromSeconds(3), () =>
                {
                    _staticApplicationUpdateInfo.CheckUpdateButtonEnable =
                    ApplicationUpdateInfo.CheckUpdateButtonEnable = true;
                    _staticApplicationUpdateInfo.CheckUpdateButtonMessage =
                    ApplicationUpdateInfo.CheckUpdateButtonMessage = string.Empty;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception in function DownloadUpdateFromServer");
                ApplicationUpdateInfo.IsDownloading = false;
                ApplicationUpdateInfo.UpdateNowButtonEnable = true;
                _staticApplicationUpdateInfo = ApplicationUpdateInfo;
            }
        }

        [RelayCommand]
        private void RestoreDefaultVersionFileServerUrl()
        {
            VersionFileServerUrl = Constants.DEFAULT_VERSION_FILE_URL;
            Log.Information($"Restore default value of VersionFileServerUrl: {VersionFileServerUrl}.");
            var isCreated = string.IsNullOrEmpty(_settingService.GetByKey(Constants.VERSION_FILE_URL)?.Value);
            _ = isCreated ? _settingService.Add(Constants.VERSION_FILE_URL, VersionFileServerUrl) : _settingService.Update(Constants.VERSION_FILE_URL, VersionFileServerUrl);
        }

        [RelayCommand]
        private void RestoreDefaultReleaseFolderServerUrl()
        {
            ReleaseFolderServerUrl = Constants.DEFAULT_RELEASE_FOLDER_URL;
            Log.Information($"Restore default value of ReleaseFolderServerUrl: {ReleaseFolderServerUrl}.");
            var isCreated = string.IsNullOrEmpty(_settingService.GetByKey(Constants.RELEASE_FOLDER_URL)?.Value);
            _ = isCreated ? _settingService.Add(Constants.RELEASE_FOLDER_URL, ReleaseFolderServerUrl) : _settingService.Update(Constants.RELEASE_FOLDER_URL, ReleaseFolderServerUrl);
        }

        [RelayCommand]
        private static void OpenMaintainenceWindow()
        {
            Log.Information($"Open Maintainence Window.");
            if (App.Current.TheMaintainenceWindow?.JustClosed ?? false)
            {
                App.Current.RenewMaintainenceWindow();
            }

            App.Current.TheMaintainenceWindow?.Show();


            if (App.Current.TheMaintainenceWindow?.WindowState == WindowState.Minimized)
            {
                App.Current.TheMaintainenceWindow.WindowState = WindowState.Normal; // Restore if minimized
            }

            // Bring to front and focus
            if (App.Current.TheMaintainenceWindow != null)
            {
                App.Current.TheMaintainenceWindow.Topmost = true;  // Make it topmost
                App.Current.TheMaintainenceWindow.Activate();     // Focus the window
                App.Current.TheMaintainenceWindow.Topmost = false; // Reset topmost
            }
        }
    }

    public partial class ApplicationUpdateInfo : ObservableObject
    {
        private bool _isChecking;
        public bool IsChecking
        {
            get => _isChecking;
            set => SetProperty(ref _isChecking, value);
        }

        private bool _isDownloading;
        public bool IsDownloading
        {
            get => _isDownloading;
            set => SetProperty(ref _isDownloading, value);
        }

        private string _updateMessage = string.Empty;
        public string UpdateMessage
        {
            get => _updateMessage;
            set => SetProperty(ref _updateMessage, value);
        }

        private string _lastUpdateMessage = string.Empty;
        public string LastUpdateMessage
        {
            get => _lastUpdateMessage;
            set => SetProperty(ref _lastUpdateMessage, value);
        }

        private string _checkUpdateButtonMessage = string.Empty;
        public string CheckUpdateButtonMessage
        {
            get => _checkUpdateButtonMessage;
            set => SetProperty(ref _checkUpdateButtonMessage, value);
        }

        private bool _needUpdateIconVisible;
        public bool NeedUpdateIconVisible
        {
            get => _needUpdateIconVisible;
            set => SetProperty(ref _needUpdateIconVisible, value);
        }

        private bool _latestIconIconVisible = true;
        public bool LatestIconIconVisible
        {
            get => _latestIconIconVisible;
            set => SetProperty(ref _latestIconIconVisible, value);
        }

        private bool _checkUpdateButtonEnable = true;
        public bool CheckUpdateButtonEnable
        {
            get => _checkUpdateButtonEnable;
            set => SetProperty(ref _checkUpdateButtonEnable, value);
        }

        private bool _checkUpdateButtonVisible = true;
        public bool CheckUpdateButtonVisible
        {
            get => _checkUpdateButtonVisible;
            set => SetProperty(ref _checkUpdateButtonVisible, value);
        }

        private bool _updateNowButtonVisible;
        public bool UpdateNowButtonVisible
        {
            get => _updateNowButtonVisible;
            set => SetProperty(ref _updateNowButtonVisible, value);
        }

        private bool _updateNowButtonTextVisible;
        public bool UpdateNowButtonTextVisible
        {
            get => _updateNowButtonTextVisible;
            set => SetProperty(ref _updateNowButtonTextVisible, value);
        }

        private bool _updateNowButtonProgressVisible;
        public bool UpdateNowButtonProgressVisible
        {
            get => _updateNowButtonProgressVisible;
            set => SetProperty(ref _updateNowButtonProgressVisible, value);
        }

        private string _updateNowButtonToolTipContent = string.Empty;
        public string UpdateNowButtonToolTipContent
        {
            get => _updateNowButtonToolTipContent;
            set => SetProperty(ref _updateNowButtonToolTipContent, value);
        }

        private bool _updateNowButtonEnable = true;
        public bool UpdateNowButtonEnable
        {
            get => _updateNowButtonEnable;
            set => SetProperty(ref _updateNowButtonEnable, value);
        }
    }
}
