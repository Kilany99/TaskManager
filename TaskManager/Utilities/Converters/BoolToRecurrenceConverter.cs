using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TaskManager.Enums;

namespace TaskManager.Utilities.Converters
{
    public class BoolToRecurrenceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is RecurrencePattern recurrence) && recurrence != RecurrencePattern.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isChecked && isChecked)
                ? RecurrencePattern.Daily // Default recurrence
                : RecurrencePattern.None;
        }
    }
}
