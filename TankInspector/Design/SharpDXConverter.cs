using System;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using SharpDX;

namespace Smellyriver.TankInspector.Design
{
	internal class SharpDxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Transform3D)
            {
                var trans = (Transform3D)value;
                var matrix = trans.Value;

                var m = new Matrix(
                    (float)matrix.M11, (float)matrix.M12, (float)matrix.M13, (float)matrix.M14,
                    (float)matrix.M21, (float)matrix.M22, (float)matrix.M23, (float)matrix.M24,
                    (float)matrix.M31, (float)matrix.M32, (float)matrix.M33, (float)matrix.M34,
                    (float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);

                return m;
            }
	        if (value is Point3D)
	        {
		        var point = (Point3D)value;
		        var v = new Vector3((float)point.X,(float)point.Y,(float)point.Z);
		        return v;
	        }
	        if (value is Vector3D)
	        {
		        var vector = (Vector3D)value;
		        var v = new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
		        return v;
	        }
	        return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
