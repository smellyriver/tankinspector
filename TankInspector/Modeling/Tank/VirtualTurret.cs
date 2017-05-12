using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualTurret : VirtualDamageableModule, ITurret
    {


        public override ModuleType Type => ModuleType.Turret;

	    public double ArmorHomogenization { get; internal set; }

        [NonSerialized]
        private Dictionary<string, IGun> _availableGuns;
        Dictionary<string, IGun> ITurret.AvailableGuns => _availableGuns;

	    private IGun _gun;
        public IGun Gun
        {
            get => _gun;
	        internal set
            {
                _gun = value;
                _availableGuns.Clear();
                _availableGuns.Add(_gun.Key, _gun);
            }
        }

        public double CamouflageFactor { get; internal set; }

        public double CircularVisionRadius { get; internal set; }

        public double FrontalArmor { get; internal set; }

        public HorizontalTraverse HorizontalTraverse { get; internal set; }

        public bool IsArmorDefined { get; internal set; }

        public double RearArmor { get; internal set; }

        public double RotationSpeed { get; internal set; }

        public double SideArmor { get; internal set; }

        public ISurveyingDevice SurveyingDevice { get; internal set; }

        public ITurretRotator TurretRotator { get; internal set; }


        public VirtualTurret()
        {
            _availableGuns = new Dictionary<string, IGun>();
        }

        protected override void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);

            _availableGuns = new Dictionary<string, IGun>();
            _availableGuns.Add(_gun.Key, _gun);
        }
    }
}
