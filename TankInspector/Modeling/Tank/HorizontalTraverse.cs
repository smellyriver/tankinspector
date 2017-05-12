using System;
using System.Globalization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class HorizontalTraverse : IDeserializable
    {
        
        public double Left { get; private set; }
        public double Right { get; private set; }

        [Stat("HorizontalTraverse", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Range => this.Left + this.Right;

	    public HorizontalTraverse(double left, double right)
        {
            this.Left = left;
            this.Right = right;
        }

        public HorizontalTraverse()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            var values = reader.ReadString();
            var valuesArray = values.Split(' ');
            this.Left = -double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
            this.Right = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return $"Gun Horizontal Traverse: {this.Left} ~ {this.Right}";
        }
    }
}
