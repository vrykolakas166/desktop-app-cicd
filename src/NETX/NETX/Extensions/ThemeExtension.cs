using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace NETX.Extensions
{
    public static class ThemeExtension
    {
        public static void SetCustomPreset(this Theme theme)
        {
            if (theme.GetBaseTheme() == BaseTheme.Dark)
            {
                // Set Dark preset
                theme.SetPrimaryColor((Color)ColorConverter.ConvertFromString("#3A96DD"));
                theme.SetSecondaryColor((Color)ColorConverter.ConvertFromString("#FF8C00"));
            }
            else
            {
                // Set Light preset
                theme.SetPrimaryColor((Color)ColorConverter.ConvertFromString("#0063B1"));
                theme.SetSecondaryColor((Color)ColorConverter.ConvertFromString("#FFB900"));
            }
        }
    }
}
