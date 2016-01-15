using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace USBLDC.Converters
{
    public class SonarStatusConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (uint)value;
            switch (status)
            {
                case 0:
                    return "停止";
                case 1:
                    return "搜索";
                case 2:
                    return "锁定";
                case 3:
                    return "错误";
            }
            return  "错误";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
