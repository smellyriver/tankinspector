using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class TankTierIconConverter : IValueConverter
    {

        public TankTierIconType Type { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return null;

                if ((int)value == -1)
                    return null;

                string prefix = "";

                if (this.Type == TankTierIconType.Light)
                    prefix = "Light";

                var tier = (int)value;
                return new BitmapImage(new Uri($"/Resources/Images/TankIcons/Tiers/{prefix}{tier}.png", UriKind.Relative));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
