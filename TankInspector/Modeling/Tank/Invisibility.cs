using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Invisibility : ISectionDeserializable
    {
        private double _moving;

        public double Moving => _moving;

	    private double _still;

        public double Still => _still;

	    private double _camouflageBonus;

        public double CamouflageBonus => _camouflageBonus;

	    private double _camouflageNetBonus;

        public double CamouflageNetBonus => _camouflageNetBonus;

	    private double _firePenalty;

        public double FirePenalty => _firePenalty;

	    public Invisibility()
        {

        }

        public Invisibility(double moving, double still, double camouflageBonus, double camouflageNetBonus, double firePenalty)
        {
            _moving = moving;
            _still = still;
            _camouflageBonus = camouflageBonus;
            _camouflageNetBonus = camouflageNetBonus;
            _firePenalty = firePenalty;
        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "moving":
                    reader.Read(out _moving);
                    return true;
                case "still":
                    reader.Read(out _still);
                    return true;
                case "camouflageBonus":
                    reader.Read(out _camouflageBonus);
                    return true;
                case "camouflageNetBonus":
                    reader.Read(out _camouflageNetBonus);
                    return true;
                case "firePenalty":
                    reader.Read(out _firePenalty);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
