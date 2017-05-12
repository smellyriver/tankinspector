using System;
using System.Windows;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class BottomMarginConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(0, 0, 0, (double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
