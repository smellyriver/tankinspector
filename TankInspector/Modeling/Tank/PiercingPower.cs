using System;
using System.Globalization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{

    [Serializable]
    public class PiercingPower : IDeserializable
    {

        [Stat("ShellPenetrationFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double P100 { get; private set; }

        [Stat("ShellPenetrationAt400mFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double P400 { get; private set; }

        public PiercingPower(double p100, double p400)
        {
            this.P100 = p100;
            this.P400 = p400;
        }

        public PiercingPower()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            var values = reader.ReadString();
            var valuesArray = values.Split(' ');
            this.P100 = double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
            this.P400 = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return $"Piercing Power: P100 = {this.P100}, P400 = {this.P400}";
        }
    }
}
