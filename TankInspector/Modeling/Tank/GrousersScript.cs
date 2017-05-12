using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class GrousersScript : EquipmentScript
    {
        private double _softGroundResistanceFactor = 1.0;
        public double SoftGroundResistanceFactor => _softGroundResistanceFactor;

	    private double _mediumGroundResistanceFactor = 1.0;
        public double MediumGroundResistanceFactor => _mediumGroundResistanceFactor;

	    public override string[] EffectiveDomains => new[] { "grousers" };

	    public GrousersScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "softGroundResistanceFactor":
                    reader.Read(out _softGroundResistanceFactor);
                    return true;
                case "mediumGroundResistanceFactor":
                    reader.Read(out _mediumGroundResistanceFactor);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }


        public override void Execute(ModificationContext sandbox, object args)
        {
            sandbox.SetValue(this.EffectiveDomains[0], "softGroundResistanceFactor", _softGroundResistanceFactor);
            sandbox.SetValue(this.EffectiveDomains[0], "mediumGroundResistanceFactor", _mediumGroundResistanceFactor);
        }
    }
}