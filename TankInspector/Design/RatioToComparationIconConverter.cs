using Smellyriver.TankInspector.DataAnalysis;
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class RatioToComparationIconConverter : IMultiValueConverter
    {
        public bool IgnoreEqual { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string key;

            if (!(values[0] is double))
                return null;

            var ratio = (double)values[0];
            var comparisonMode = (ComparisonMode)values[1];

            if (comparisonMode == ComparisonMode.NotComparable)
                return null;

            var plain = comparisonMode == ComparisonMode.Plain;
            if (ratio < -0.667)
                key = plain ? "Lower3" : "Worse3";
            else if (ratio < -0.333)
                key = plain ? "Lower2" : "Worse2";
            else if (ratio < -0.001)
                key = plain ? "Lower1" : "Worse1";
            else if (ratio < 0.001)
            {
	            if (IgnoreEqual)
                    return null;
	            key = plain ? "PlainEquals" : "Equals";
            }
            else if (ratio < 0.333)
                key = plain ? "Higher1" : "Better1";
            else if (ratio < 0.667)
                key = plain ? "Higher2" : "Better2";
            else
                key = plain ? "Higher3" : "Better3";

            return new BitmapImage(new Uri($"/Resources/Images/UIElements/Comparation{key}.png", UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
