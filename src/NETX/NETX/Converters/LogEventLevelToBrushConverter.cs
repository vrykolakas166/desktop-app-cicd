using Serilog.Events;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NETX.Converters;

public class LogEventLevelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            LogEventLevel.Verbose => new SolidColorBrush(Colors.Gray),
            LogEventLevel.Debug => new SolidColorBrush(Colors.DarkGray),
            LogEventLevel.Warning => new SolidColorBrush(Colors.Goldenrod),
            LogEventLevel.Error => new SolidColorBrush(Colors.HotPink),
            LogEventLevel.Fatal => new SolidColorBrush(Colors.Red),
            LogEventLevel.Information or _ => new SolidColorBrush(Colors.DeepSkyBlue),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
