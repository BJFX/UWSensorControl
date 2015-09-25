using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace USBLDC.Converters
{
    public class WorkAreaHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SystemParameters.WorkArea.Height;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
