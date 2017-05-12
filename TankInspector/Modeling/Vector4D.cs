using System;
using System.Globalization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Vector4D : IDeserializable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public Vector4D()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            var values = reader.ReadString();
            var valuesArray = values.Split(' ');

            if (valuesArray.Length > 0)
            {
                this.X = double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
                if (valuesArray.Length > 1)
                {
                    this.Y = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
                    if (valuesArray.Length > 2)
                    {
                        this.Z = double.Parse(valuesArray[2], CultureInfo.InvariantCulture);
                        if (valuesArray.Length > 3)
                            this.W = double.Parse(valuesArray[3], CultureInfo.InvariantCulture);
                    }
                }
            }


        }

    }
}
