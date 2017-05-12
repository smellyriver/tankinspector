using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class RemovedRpmLimiterScript : ConsumableScript
    {
        private double _enginePowerFactor = 1.0;
        public double EnginePowerFactor => _enginePowerFactor;

	    private double _engineHpLossPerSecond = 0.0;
        public double EngineHpLossPerSecond => _engineHpLossPerSecond;

	    public override string[] EffectiveDomains => new[] { "removedRpmLimiter" };


	    public RemovedRpmLimiterScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "enginePowerFactor":
                    reader.Read(out _enginePowerFactor);
                    return true;
                case "engineHpLossPerSecond":
                    reader.Read(out _engineHpLossPerSecond);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext context, object args)
        {
            context.SetValue(this.EffectiveDomains[0], "enginePowerFactor", _enginePowerFactor);
            context.SetValue(this.EffectiveDomains[0], "engineHpLossPerSecond", _engineHpLossPerSecond);
        }
    }
}