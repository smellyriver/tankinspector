using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal interface ISectionDeserializable : IDeserializable
    {
        bool DeserializeSection(string name, XmlReader reader);
        bool IsWrapped { get; }
    }
}
