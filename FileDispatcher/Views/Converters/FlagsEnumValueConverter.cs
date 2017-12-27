using System;
using System.Globalization;
using System.Windows.Data;

namespace FileDispatcher.Views.Converters
{
    public class FlagsEnumValueConverter : IValueConverter
    {
        private int targetValue;

        public FlagsEnumValueConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int mask = (int)parameter;
            targetValue = (int)value;

            return ((mask & targetValue) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            targetValue ^= (int)parameter;
            return Enum.Parse(targetType, targetValue.ToString());
        }
    }
}
