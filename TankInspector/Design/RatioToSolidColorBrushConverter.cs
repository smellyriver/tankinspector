using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Design
{
	internal class RatioToSolidColorBrushConverter : IValueConverter
    {
        public Color BestColor { get; set; }
        public Color WorstColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ratio = System.Convert.ToSingle(value);
            var normalizedRatio = (ratio + 1) / 2;
            return new SolidColorBrush(WorstColor + Color.Multiply((BestColor - WorstColor), normalizedRatio));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
