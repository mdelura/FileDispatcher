using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace FileDispatcher.Views.Converters
{
    class ValidationErrorsToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var validationErrors = values[0] as IEnumerable<ValidationError>;

            if (validationErrors == null)
                return null;

            var errorContents = validationErrors
                .Select(e => (e.ErrorContent as ValidationResult)?.ErrorContent ?? e.ErrorContent);

            return string.Join("\r\n", errorContents);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
