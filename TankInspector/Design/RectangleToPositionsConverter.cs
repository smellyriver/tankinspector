using System;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace Smellyriver.TankInspector.Design
{
    public class RectangleToPositionsConverter : IMultiValueConverter
    {
	    private const double Scale = 142;
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is double && values[1] is double)
            {
                var width = (double)values[0];
                var height = (double)values[1];
                width /= Scale;
                height /= Scale;

                var points = new Point3DCollection(4);
                points.Add(new Point3D(-width, height, 0));
                points.Add(new Point3D(-width, -height, 0));
                points.Add(new Point3D(width, -height, 0));
                points.Add(new Point3D(width, height, 0));
                return points;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
