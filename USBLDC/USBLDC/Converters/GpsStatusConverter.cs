using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace USBLDC.Converters
{
    public class GpsStatusConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (uint) value;
            switch (status)
            {
                case 0:
                    return "未定位";
                case 1:
                    return "非差分定位";
                case 2:
                    return "差分定位";
                case 3:
                    return "正在估算";
            }
            return "未定位";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
