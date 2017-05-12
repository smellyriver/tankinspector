using System;
using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{

    [Serializable]
    internal class WheelInfo : LodVisual
    {

        public List<WheelDefinition> Wheels { get; set; }
        public List<WheelGroupDefinition> Groups { get; set; }
        public List<string> DrivingWheelNames { get; set; }

        public WheelInfo()
        {
            this.Wheels = new List<WheelDefinition>();
            this.Groups = new List<WheelGroupDefinition>();
            this.DrivingWheelNames = new List<string>();
        }


        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "wheel":
                    WheelDefinition wheel;
                    reader.Read(out wheel);
                    this.Wheels.Add(wheel);
                    return true;
                case "group":
                    WheelGroupDefinition group;
                    reader.Read(out group);
                    this.Groups.Add(group);
                    return true;

                default:
                    return base.DeserializeSection(name, reader);
            }
        }

    }
}
