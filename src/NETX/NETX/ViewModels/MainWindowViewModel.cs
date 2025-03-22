using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NETX.Helpers;
using NETX.Views.Pages;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace NETX.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly Dictionary<string, Type> _pageMappings = new()
        {
            { "Home", typeof(HomePage) },
            { "Image converter", typeof(ImagePage) } ,
            { "Settings", typeof(SettingPage) } ,
            // Add other page mappings here
        };

        [ObservableProperty]
        private ObservableCollection<KeyValuePair<string, string>>? _suggestionList;

        private readonly List<KeyValuePair<string, string>>? _originalSuggestionList;

        [ObservableProperty]
        private string _suggestionText = string.Empty;

        [ObservableProperty]
        private bool _canCopySuggestionText = true;

        [ObservableProperty]
        private Page _currentPage = new HomePage();

        public MainWindowViewModel()
        {
            _originalSuggestionList = new List<KeyValuePair<string, string>>(GetUtilities());
        }

        [RelayCommand]
        private void ClearSuggestionText()
        {
            SuggestionText = string.Empty;
        }

        [RelayCommand]
        private void QuickNavigateToHome()
        {
            CurrentPage = new HomePage();
            if (App.Current.Window != null) App.Current.Window.Title = "Home";
        }

        [RelayCommand]
        private void QuickNavigateToSetting()
        {
            CurrentPage = new SettingPage();
            if (App.Current.Window != null) App.Current.Window.Title = "Settings";
        }

        [RelayCommand(CanExecute = nameof(CanCopySuggestionText))]
        private void CopySuggestionText()
        {
            Clipboard.SetText(SuggestionText);
            // Delay for 1.5s
            CanCopySuggestionText = false;
            TaskHelper.RunAfter(TimeSpan.FromMilliseconds(1500), () =>
            {
                CanCopySuggestionText = true;
            });
        }

        partial void OnSuggestionTextChanged(string value)
        {
            if (_originalSuggestionList is not null && value is not null)
            {
                var searchResult = _originalSuggestionList.Where(x => IsMatch(x.Value, value));
                SuggestionList = new ObservableCollection<KeyValuePair<string, string>>(searchResult);
            }

            if (SuggestionList?.Any(s => s.Value.Equals(value)) ?? false)
            {
                CurrentPage = (Activator.CreateInstance(_pageMappings[value ?? "Home"]) as Page ?? new HomePage());
                if (App.Current.Window != null) App.Current.Window.Title = value ?? "Home";
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetUtilities()
        {
            return [
                new KeyValuePair<string, string>("HOME_PAGE", "Home"),
                new KeyValuePair<string, string>("IMAGE_CONVERTER", "Image converter"),
                new KeyValuePair<string, string>("SETTING_PAGE", "Settings"),
            ];
        }

        private static bool IsMatch(string item, string currentText)
        {
            return item.Contains(currentText, StringComparison.OrdinalIgnoreCase);
        }
    }
}
