using System;
using System.Xml.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    public class RegularBattleTierRule
    {
        [XmlAttribute]
        public int VehicleTier { get; set; }
        [XmlAttribute]
        public TankClass Class { get; set; }

        public BattleTierSpan BattleTier { get; set; }
    }
}
