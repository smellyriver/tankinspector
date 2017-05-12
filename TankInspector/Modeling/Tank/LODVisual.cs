using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class LodVisual : ISectionDeserializable
    {
        private string _lodDist;
        public string LodDist
        {
            get => _lodDist;
	        set => _lodDist = value;
        }


        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            return this.DeserializeSection(name, reader);
        }

        protected virtual bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "lodDist":
                    reader.Read(out _lodDist);
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool IsWrapped => false;

	    bool ISectionDeserializable.IsWrapped => this.IsWrapped;
    }
}
