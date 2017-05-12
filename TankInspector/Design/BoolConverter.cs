using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class BoolConverter : IValueConverter
    {

        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
	        if ((bool)value)
                return this.TrueValue;
	        return this.FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
