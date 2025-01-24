using System;
using System.Globalization;
using System.Windows.Data;
using TaskManager.Enums;

namespace TaskManager.Utilities
{
    public class PriorityEnumToValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetValues(typeof(Priority)); // Return all enum values
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }
}
