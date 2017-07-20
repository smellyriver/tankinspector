using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class ShellTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch((ShellType)value)
            {
                case ShellType.AP:
                    return App.GetLocalizedString("AP");
                case ShellType.APCR:
                    return App.GetLocalizedString("APCR");
                case ShellType.APHE:
                    return App.GetLocalizedString("APHE");
                case ShellType.HE:
                    return App.GetLocalizedString("HE");
                case ShellType.HEAT:
                    return App.GetLocalizedString("HEAT");
                case ShellType.PremiumHE:
                    return App.GetLocalizedString("PremiumHE");
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
