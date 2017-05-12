using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class FuelTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((FuelType)value)
            {
                case FuelType.Diesel:
                    return App.GetLocalizedString("Diesel");
                case FuelType.Gasoline:
                    return App.GetLocalizedString("Gasoline");
                default:
                    return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
