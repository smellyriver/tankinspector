using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class WheelDefinition : ISectionDeserializable
    {
        public HandSide Side { get; set; }

        private double _radius;
        public double Radius
        {
            get => _radius;
	        set => _radius = value;
        }

        private string _name;
        public string Name
        {
            get => _name;
	        set => _name = value;
        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "isLeft":
                    bool isLeft;
                    reader.Read(out isLeft);
                    this.Side = isLeft ? HandSide.Left : HandSide.Right;
                    return true;
                case "radius":
                    reader.Read(out _radius);
                    return true;
                case "name":
                    reader.Read(out _name);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
