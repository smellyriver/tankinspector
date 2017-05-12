using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    public interface IDeserializable
    {
        void Deserialize(XmlReader reader);
    }
}
