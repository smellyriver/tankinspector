using System;
using System.Globalization;
using System.Windows.Media.Media3D;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Vector3D: IDeserializable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            var values = reader.ReadString();
            var valuesArray = values.Split(' ');
            this.X = double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
            this.Y = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
            this.Z = double.Parse(valuesArray[2], CultureInfo.InvariantCulture);
        }

        public Point3D ToPoint3D()
        {
            return new Point3D(this.X, this.Y, this.Z);
        }
    }
}
