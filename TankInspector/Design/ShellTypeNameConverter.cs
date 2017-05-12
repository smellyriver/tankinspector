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
                case ShellType.Ap:
                    return App.GetLocalizedString("AP");
                case ShellType.Apcr:
                    return App.GetLocalizedString("APCR");
                case ShellType.Aphe:
                    return App.GetLocalizedString("APHE");
                case ShellType.He:
                    return App.GetLocalizedString("HE");
                case ShellType.Heat:
                    return App.GetLocalizedString("HEAT");
                case ShellType.PremiumHe:
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
