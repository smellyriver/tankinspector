using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class TankClassIconConverter : IMultiValueConverter
    {
        public TankClassConversionType Type { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 0)
                return null;

            if (!(values[0] is TankClass))
                return null;

            var tankClass = (TankClass)values[0];
            var isElite = values.Length > 1 && values[1] is bool ? (bool)values[1] : this.Type == TankClassConversionType.Elite;

            var prefix = tankClass.ToString();
            string postfix;

            switch (this.Type)
            {
                case TankClassConversionType.Normal:
                    postfix = ".png";
                    break;
                case TankClassConversionType.Small:
                    postfix = "Small.png";
                    break;
                case TankClassConversionType.Auto:
                    if (isElite)
                        postfix = "Elite.png";
                    else
                        postfix = ".png";
                    break;
                case TankClassConversionType.Elite:
                    postfix = "Elite.png";
                    break;
                default:
                    return null;
            }

            return new BitmapImage(new Uri($"/Resources/Images/TankIcons/Classes/{prefix}{postfix}", UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
