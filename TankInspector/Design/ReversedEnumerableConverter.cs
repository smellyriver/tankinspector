using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class ReversedEnumerableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((IEnumerable)value).OfType<object>().Reverse();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((IEnumerable)value).OfType<object>().Reverse();
        }
    }
}
