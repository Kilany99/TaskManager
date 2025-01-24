using System.Globalization;
using System.Windows.Data;

namespace TaskManager.Utilities
{
    public class EnumToItemsSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type enumType)
                return Enum.GetValues(enumType);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}