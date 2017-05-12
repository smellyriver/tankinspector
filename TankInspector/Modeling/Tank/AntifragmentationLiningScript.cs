using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class AntifragmentationLiningScript : EquipmentScript
    {
        private double _antifragmentationLiningFactor = 1.0;
        public double AntifragmentationLiningFactor => _antifragmentationLiningFactor;

	    private double _increaseCrewChanceToEvadeHit = 0.0;
        public double IncreaseCrewChanceToEvadeHit => _increaseCrewChanceToEvadeHit;

	    public override string[] EffectiveDomains => new[] { "antifragmentationLining" };

	    public AntifragmentationLiningScript(Database database)
            : base(database)
        {

        }


        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "antifragmentationLiningFactor":
                    reader.Read(out _antifragmentationLiningFactor);
                    return true;
                case "increaseCrewChanceToEvadeHit":
                    reader.Read(out _increaseCrewChanceToEvadeHit);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext sandbox, object args)
        {
            sandbox.SetValue(this.EffectiveDomains[0], "antifragmentationLiningFactor", _antifragmentationLiningFactor);
            sandbox.SetValue(this.EffectiveDomains[0], "increaseCrewChanceToEvadeHit", _increaseCrewChanceToEvadeHit);
        }
    }
}
