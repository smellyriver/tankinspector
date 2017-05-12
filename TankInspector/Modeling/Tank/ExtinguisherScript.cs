using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class ExtinguisherScript : ConsumableScript
    {
        private double _fireStartingChanceFactor = 1.0;
        public double FireStartingChanceFactor => _fireStartingChanceFactor;

	    private bool _autoactivate;
        public bool Autoactivate => _autoactivate;

	    public override string[] EffectiveDomains => new[] { "extinguisher" };

	    public ExtinguisherScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "fireStartingChanceFactor":
                    reader.Read(out _fireStartingChanceFactor);
                    return true;
                case "autoactivate":
                    reader.Read(out _autoactivate);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext context, object args)
        {
            context.SetValue(this.EffectiveDomains[0], "fireStartingChanceFactor", _fireStartingChanceFactor);
            context.SetValue(this.EffectiveDomains[0], "autoactivate", _autoactivate ? 1.0 : 0.0);
        }
    }
}
