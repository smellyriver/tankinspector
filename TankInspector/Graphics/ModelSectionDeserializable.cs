using System.Xml;
using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.Graphics
{
    public abstract class ModelSectionDeserializable : ISectionDeserializable
    {
        public bool IsWrapped => false;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        public abstract bool DeserializeSection(string name, XmlReader reader);
    }
}
