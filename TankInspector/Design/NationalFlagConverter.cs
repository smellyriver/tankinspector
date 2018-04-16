using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class NationalFlagConverter : IValueConverter
    {
        public NationalFlagType Type { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string prefix, postfix;

            switch ((string)value)
            {
                case "ussr":
                    prefix = "Soviet";
                    break;
                case "usa":
                    prefix = "America";
                    break;
                case "germany":
                    prefix = "Germany";
                    break;
                case "uk":
                    prefix = "British";
                    break;
                case "china":
                    prefix = "China";
                    break;
                case "japan":
                    prefix = "Japan";
                    break;
                case "france":
                    prefix = "France";
                    break;
                case "czech":
                    prefix = "Czech";
                    break;
                case "sweden":
                    prefix = "Sweden";
                    break;
                case "poland":
                    prefix = "Poland";
                    break;
                case "italy":
                    prefix = "Italy";
                    break;
                default:
                    return null;
            }

            switch (this.Type)
            {
                case NationalFlagType.LargeBlurred:
                    postfix = "LargeBlurred.png";
                    break;
                case NationalFlagType.Small:
                    postfix = "Small.bmp";
                    break;
                default:
                    return null;
            }

            return new BitmapImage(new Uri($"/Resources/Images/Flags/{prefix}{postfix}", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
