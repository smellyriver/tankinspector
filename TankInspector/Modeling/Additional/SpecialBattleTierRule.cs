using System;
using System.Xml.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    public class SpecialBattleTierRule
    {
        [XmlAttribute]
        public string Vehicle { get; set; }

        public BattleTierSpan BattleTier { get; set; }
    }
}
