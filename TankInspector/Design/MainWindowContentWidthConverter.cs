using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class MainWindowContentWidthConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var actualWidth = (double)values[0];
            if (actualWidth > 1280)
                return actualWidth;
	        return 1280.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
