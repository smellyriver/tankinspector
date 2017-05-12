using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class CamouflageNetScript : EquipmentScript
    {

        public static readonly Dictionary<TankClass, double> AdditiveCamouflageValues;

        static CamouflageNetScript()
        {
            AdditiveCamouflageValues = new Dictionary<TankClass, double>();
            AdditiveCamouflageValues.Add(TankClass.HeavyTank, 0.05);
            AdditiveCamouflageValues.Add(TankClass.SelfPropelledGun, 0.05);
            AdditiveCamouflageValues.Add(TankClass.MediumTank, 0.1);
            AdditiveCamouflageValues.Add(TankClass.LightTank, 0.1);
            AdditiveCamouflageValues.Add(TankClass.TankDestroyer, 0.15);
        }

        private double _activateWhenStillSec;
        public double ActivateWhenStillSec => _activateWhenStillSec;
	    public override string[] EffectiveDomains => new[] { "camouflageNet" };

	    public CamouflageNetScript(Database database)
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
                default:
                    return base.DeserializeSection(name, reader);
            }
        }


        public override void Execute(ModificationContext sandbox, object args)
        {
            sandbox.SetValue(this.EffectiveDomains[0], "activateWhenStillSec", _activateWhenStillSec);
        }
    }
}
