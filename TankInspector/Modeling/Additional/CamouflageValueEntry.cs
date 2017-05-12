using System.Xml.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [XmlRoot("Entry")]
    public class CamouflageValueEntry
    {
        [XmlAttribute]
        public string Vehicle { get; set; }

        public CamouflageValue Value { get; set; }
    }
}
