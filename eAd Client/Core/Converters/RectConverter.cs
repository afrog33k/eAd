using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ClientApp.Core.Converters
{

    class RectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double[] doubles = values.OfType<double>().ToArray();

            if (doubles.Length == 2)
            {
                return new Rect(0, 0, doubles[0], doubles[1] + 2);
            }
            else
                return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
