using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class StereoscopeScript : EquipmentScript
    {
        private double _activateWhenStillSec;
        public double ActivateWhenStillSec => _activateWhenStillSec;

	    private double _circularVisionRadiusFactor = 1.0;
        public double CircularVisionRadiusFactor => _circularVisionRadiusFactor;

	    public override string[] EffectiveDomains => new[] { "stereoscope" };

	    public StereoscopeScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "activateWhenStillSec":
                    reader.Read(out _activateWhenStillSec);
                    return true;
                case "circularVisionRadiusFactor":
                    reader.Read(out _circularVisionRadiusFactor);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override void Execute(ModificationContext sandbox, object args)
        {
            sandbox.SetValue(this.EffectiveDomains[0], "activateWhenStillSec", _activateWhenStillSec);
            sandbox.SetValue(this.EffectiveDomains[0], "circularVisionRadiusFactor", _circularVisionRadiusFactor);
        }
    }
}
