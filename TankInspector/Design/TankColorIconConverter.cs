using Smellyriver.TankInspector.Modeling;
using Smellyriver.TankInspector.UIComponents;
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
    internal class TankColorIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            
            if (value == null)
                return null;

            string prefix="";

            if (value is TankViewModel)
            {
                var tankVm = (TankViewModel)value;
            }
            else
            {
                prefix = value.ToString();
            }

            return new BitmapImage(new Uri($"/Resources/Images/TankIcons/Classes/{prefix}Color.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
