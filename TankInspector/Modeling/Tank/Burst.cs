using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Burst : ISectionDeserializable
    {
        private int _count;
        [Stat("GunBurstCountFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public int Count => _count;

	    private double _rate;
        [Stat("GunBurstRateOfFireFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Rate => _rate;

	    public Burst(int count, double rate)
        {
            _count = count;
            _rate = rate;
        }

        public Burst()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "count":
                    reader.Read(out _count);
                    return true;
                case "rate":
                    reader.Read(out _rate);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;

	    public override string ToString()
        {
            return $"Burst: Count = {this.Count}, Rate = {this.Rate}";
        }
    }
}
