using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class WheelGroupDefinition : ISectionDeserializable
    {

        public HandSide Side { get; set; }

        private string _wheelNameTemplate;
        public string WheelNameTemplate
        {
            get => _wheelNameTemplate;
	        set => _wheelNameTemplate = value;
        }

        private double _radius;
        public double Radius
        {
            get => _radius;
	        set => _radius = value;
        }

        private int _startIndex;
        public int StartIndex
        {
            get => _startIndex;
	        set => _startIndex = value;
        }

        private int _count;
        public int Count
        {
            get => _count;
	        set => _count = value;
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
                case "template":
                    reader.Read(out _wheelNameTemplate);
                    return true;
                case "radius":
                    reader.Read(out _radius);
                    return true;
                case "startIndex":
                    reader.Read(out _startIndex);
                    return true;
                case "count":
                    reader.Read(out _count);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
