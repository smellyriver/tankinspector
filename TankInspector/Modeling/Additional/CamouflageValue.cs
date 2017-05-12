using System;
using System.Xml.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    [XmlRoot("Camoflague")]
    public class CamouflageValue
    {
        [XmlAttribute]
        public double Stationary { get; set; }
        [XmlAttribute]
        public double Moving { get; set; }
        [XmlAttribute]
        public double Firing { get; set; }

        public CamouflageValue()
        {

        }

        public CamouflageValue(double stationary, double moving, double firing)
        {
            this.Stationary = stationary;
            this.Moving = moving;
            this.Firing = firing;
        }
    }
}
