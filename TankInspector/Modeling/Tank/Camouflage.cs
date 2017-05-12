using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Camouflage : ISectionDeserializable
    {
        private double _priceFactor;
        public double PriceFactor
        {
            get => _priceFactor;
	        set => _priceFactor = value;
        }
            
        private Vector4D _tiling;
        public Vector4D Tiling
        {
            get => _tiling;
	        set => _tiling = value;
        }

        private string _exclusionMask;
        public string ExclusionMask
        {
            get => _exclusionMask;
	        set => _exclusionMask = value;
        }


        public Camouflage(double priceFactor)
        {
            _priceFactor = priceFactor;
        }

        public Camouflage()
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
                case "priceFactor":
                    reader.Read(out _priceFactor);
                    return true;
                case "tiling":
                    reader.Read(out _tiling);
                    return true;
                case "exclusionMask":
                    reader.Read(out _exclusionMask);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
