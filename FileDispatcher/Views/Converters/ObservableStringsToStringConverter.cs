using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileDispatcher.Views.Converters
{
    class ObservableStringsToStringConverter : IValueConverter
    {
        const string defaultSeparator = ", ";
        static string[] validSeparators;


        static ObservableStringsToStringConverter()
        {
            validSeparators = new string[]
            {
                ", ",
                "; ",
                ",",
                ";"
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? 
                string.Join(defaultSeparator, (ObservableCollection<string>)value) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? 
                new ObservableCollection<string>(value.ToString().Split(validSeparators, StringSplitOptions.RemoveEmptyEntries))
                : null;
        }
    }
}
