using System.Globalization;
using System.Windows.Data;

namespace MES.Presentation.UI.Converters
{
    public class BoolToMenuStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (bool)value ? "Expanded" : "Collapsed";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
