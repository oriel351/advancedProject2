using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
//add reference??
using System.Windows.Data;

namespace SharedData
{
    class converenumlog
    {
        class LogEnumToColor : IValueConverter
        {
            
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (targetType.Name != "Brush")
                {
                    throw new InvalidOperationException("Must convert to a brush!");
                }

                string type = (string)value;
                switch (type)
                {
                    case "INFO":
                        return System.Windows.Media.Brushes.LightSeaGreen;
                    case "WARNING":
                        return System.Windows.Media.Brushes.DarkOrange;
                    case "ERROR":
                        return System.Windows.Media.Brushes.PaleVioletRed;
                    default:
                        return System.Windows.Media.Brushes.White;
                }

            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    }
}
