using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class EnhancedSuspensionScript : EquipmentScript
    {
        private double _chassisMaxLoadFactor = 1.0;
        public double ChassisMaxLoadFactor => _chassisMaxLoadFactor;

	    private double _chassisHealthFactor = 1.0;
        public double ChassisHealthFactor => _chassisHealthFactor;

	    private double _vehicleByChassisDamageFactor = 1.0;
        public double VehicleByChassisDamageFactor => _vehicleByChassisDamageFactor;

	    public override string[] EffectiveDomains => new[] { "enhancedSuspension" };

	    public EnhancedSuspensionScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "chassisMaxLoadFactor":
                    reader.Read(out _chassisMaxLoadFactor);
                    return true;
                case "chassisHealthFactor":
                    reader.Read(out _chassisHealthFactor);
                    return true;
                case "vehicleByChassisDamageFactor":
                    reader.Read(out _vehicleByChassisDamageFactor);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }



        public override void Execute(ModificationContext sandbox, object args)
        {
            sandbox.SetValue(this.EffectiveDomains[0], "chassisMaxLoadFactor", _chassisMaxLoadFactor);
            sandbox.SetValue(this.EffectiveDomains[0], "chassisHealthFactor", _chassisHealthFactor);
            sandbox.SetValue(this.EffectiveDomains[0], "vehicleByChassisDamageFactor", _vehicleByChassisDamageFactor);
        }
    }
}
