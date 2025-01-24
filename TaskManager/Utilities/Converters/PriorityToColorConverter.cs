using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Enums;

namespace TaskManager.Utilities
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Priority)value switch
            {
                Priority.Low => Brushes.Green,
                Priority.Medium => Brushes.Orange,
                Priority.High => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}