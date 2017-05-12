using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Clip : ISectionDeserializable
    {
        private int _count;
        [Stat("GunClipCountFullName", DataAnalysis.ComparisonMode.Plain)]
        public int Count => _count;

	    private double _rate;
        public double Rate => _rate;

	    [Stat("GunClipReloadTimeFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double ReloadTime => 60.0 / _rate;

	    public Clip(int count, double rate)
        {
            _count = count;
            _rate = rate;
        }

        public Clip()
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
            return $"Clip: Count = {this.Count}, ReloadTime = {this.ReloadTime}";
        }
    }
}
