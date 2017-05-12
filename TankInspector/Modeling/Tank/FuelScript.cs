using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class FuelScript : ConsumableScript
    {
        private double _enginePowerFactor = 1.0;
        public double EnginePowerFactor => _enginePowerFactor;

	    private double _turretRotationSpeedFactor = 1.0;
        public double TurretRotationSpeedFactor => _turretRotationSpeedFactor;

	    public override string[] EffectiveDomains => new[] { "fuel" };

	    public FuelScript(Database database)
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
                case "turretRotationSpeedFactor":
                    reader.Read(out _turretRotationSpeedFactor);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext context, object args)
        {
            context.SetValue(this.EffectiveDomains[0], "enginePowerFactor", _enginePowerFactor);
            context.SetValue(this.EffectiveDomains[0], "turretRotationSpeedFactor", _turretRotationSpeedFactor);
        }
    }
}
