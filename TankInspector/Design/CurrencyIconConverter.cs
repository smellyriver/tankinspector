using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class CurrencyIconConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            return new BitmapImage(new Uri($"/Resources/Images/GameElements/{value.ToString()}.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
