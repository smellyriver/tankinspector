using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class RepairkitScript : ConsumableScript
    {
        private double _bonusValue = 0.0;
        public double BonusValue => _bonusValue;

	    private bool _repairAll;
        public bool RepairAll => _repairAll;
	    public override string[] EffectiveDomains => new[] { "repairkit" };

	    public RepairkitScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "bonusValue":
                    reader.Read(out _bonusValue);
                    return true;
                case "repairAll":
                    reader.Read(out _repairAll);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext context, object args)
        {
            context.SetValue(this.EffectiveDomains[0], "bonusValue", _bonusValue);
            context.SetValue(this.EffectiveDomains[0], "repairAll", _repairAll ? 1.0 : 0.0);
        }
    }
}
