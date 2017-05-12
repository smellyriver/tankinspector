using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class HalfConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * 2;
        }

    }
}
