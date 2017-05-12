using Smellyriver.TankInspector.Modeling;
using Smellyriver.TankInspector.UIComponents;
using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal class TankClassNameConverter : IValueConverter
    {
        public TankClassConversionType Type { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;


            if (value == null)
                return null;

            string prefix, postfix;

            if (value is TankViewModel)
            {
                var tankVm = (TankViewModel)value;

                postfix = App.GetLocalizedString(tankVm.Tank.Class.ToString());

                switch (this.Type)
                {
                    case TankClassConversionType.Normal:
                    case TankClassConversionType.Small:
                        prefix = "";
                        break;
                    case TankClassConversionType.Auto:
                        if (tankVm.IsElite)
                            prefix = App.GetLocalizedString("Elite") + " ";
                        else
                            prefix = "";
                        break;
                    case TankClassConversionType.Elite:
                        prefix = App.GetLocalizedString("Elite");
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                var tankClass = (TankClass)value;
                postfix = App.GetLocalizedString(tankClass.ToString());

                switch (this.Type)
                {
                    case TankClassConversionType.Normal:
                    case TankClassConversionType.Auto:
                    case TankClassConversionType.Small:
                        prefix = "";
                        break;
                    case TankClassConversionType.Elite:
                        prefix = App.GetLocalizedString("Elite");
                        break;
                    default:
                        return null;
                }
            }

            return prefix + postfix;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
