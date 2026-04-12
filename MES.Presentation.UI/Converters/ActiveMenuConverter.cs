using System.Globalization;
using System.Windows.Data;

namespace MES.Presentation.UI.Converters;

public class ActiveMenuConverter : IValueConverter
{
    public object Convert(object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        // value = ActiveMenu from ViewModel
        // parameter = "Overview", "Material", etc.
        return string.Equals(value.ToString(),
            parameter.ToString(),
            StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}