using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using NETX.Extensions;
using System.Windows;

namespace NETX.Views.Layout
{
    public partial class ControlBarViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLightTheme = false;

        public ControlBarViewModel()
        {
            _isLightTheme = true;
        }

        private void ToggleTheme()
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(!IsLightTheme ? BaseTheme.Dark : BaseTheme.Light);
            theme.SetCustomPreset();
            paletteHelper.SetTheme(theme);
        }

        partial void OnIsLightThemeChanged(bool value)
        {
            ToggleTheme();
        }

        [RelayCommand]
        private static void Minimize(ControlBar ctrl)
        {
            Window parentWindow = Window.GetWindow(ctrl);
            parentWindow.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private static void Maximize(ControlBar ctrl)
        {
            Window parentWindow = Window.GetWindow(ctrl);
            if(parentWindow.WindowState == WindowState.Maximized)
            {
                parentWindow.WindowState = WindowState.Normal;
            }
            else if(parentWindow.WindowState == WindowState.Normal)
            {
                parentWindow.WindowState = WindowState.Maximized;
            }
        }

        [RelayCommand]
        private static void Close(ControlBar ctrl)
        {
            Window parentWindow = Window.GetWindow(ctrl);
            parentWindow.Close();
        }
    }
}
