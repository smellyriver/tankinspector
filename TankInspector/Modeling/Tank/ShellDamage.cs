using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    public class ShellDamage : ISectionDeserializable
    {
        private double _armor;
        [Stat("ShellArmorDamageFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Armor => _armor;

	    private double _devices;
        [Stat("ShellModuleDamageFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Devices => _devices;


	    public ShellDamage(double armor, double devices)
        {
            _armor = armor;
            _devices = devices;
        }

        public ShellDamage()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "armor":
                    reader.Read(out _armor);
                    return true;
                case "devices":
                    reader.Read(out _devices);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;

	    public override string ToString()
        {
            return $"Shell Damage: Armor = {this.Armor}, Devices = {this.Devices}";
        }

    }
}
