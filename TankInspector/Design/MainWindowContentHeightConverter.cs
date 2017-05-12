using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class MainWindowContentHeightConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var actualWidth = (double)values[0];
            var actualHeight = (double)values[1];
            if (actualWidth > 1280)
                return actualHeight;
	        return Math.Max(100, actualHeight * 1280 / actualWidth);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
