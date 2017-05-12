using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Turret : DamageableModule, IHasArmor, ITurret
    {
        public override ModuleType Type => ModuleType.Turret;

	    private HorizontalTraverse _horizontalTraverse;

        [ExpandableStat]
        public HorizontalTraverse HorizontalTraverse => _horizontalTraverse;

	    private double _rotationSpeed;

        [Stat("TurretRotationSpeedFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double RotationSpeed => _rotationSpeed;

	    private double _armorHomogenization;
        public double ArmorHomogenization => _armorHomogenization;

	    [StatSubItem("TurretRotator")]
        public TurretRotator TurretRotator { get; private set; }

	    ITurretRotator ITurret.TurretRotator => this.TurretRotator;

	    [StatSubItem("SurveyingDevice")]
        public SurveyingDevice SurveyingDevice { get; private set; }

	    ISurveyingDevice ITurret.SurveyingDevice => this.SurveyingDevice;

	    private double _circularVisionRadius;
        [Stat("TurretCircularVisionRadiusFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double CircularVisionRadius => _circularVisionRadius;

	    public Armor Armor { get; private set; }

	    private Gun[] _availableGunsArray;

        [NonSerialized]
        private Dictionary<string, Gun> _availableGuns;

        [StatGroup("Guns")]
        public Dictionary<string, Gun> AvailableGuns => _availableGuns;

	    Dictionary<string, IGun> ITurret.AvailableGuns
        {
            get { return _availableGuns.ToDictionary(g => g.Key, g => (IGun)g.Value); }
        }

        private PrimaryArmorKeys _primaryArmorKeys;



        private double _camouflageFactor;
        public double CamouflageFactor => _camouflageFactor;

	    public bool IsArmorDefined => _primaryArmorKeys != null
	                                  && this.Armor.ArmorGroups.ContainsKey(_primaryArmorKeys.Front)
	                                  && this.Armor.ArmorGroups[_primaryArmorKeys.Front].Value > 0
	                                  && this.Armor.ArmorGroups.ContainsKey(_primaryArmorKeys.Side)
	                                  && this.Armor.ArmorGroups[_primaryArmorKeys.Side].Value > 0
	                                  && this.Armor.ArmorGroups.ContainsKey(_primaryArmorKeys.Rear)
	                                  && this.Armor.ArmorGroups[_primaryArmorKeys.Rear].Value > 0;

	    [Stat("TurretFrontalArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double FrontalArmor => _primaryArmorKeys == null ? 0.0 : this.Armor.GetArmorValue(_primaryArmorKeys.Front);

	    [Stat("TurretSideArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double SideArmor => _primaryArmorKeys == null ? 0.0 : this.Armor.GetArmorValue(_primaryArmorKeys.Side);

	    [Stat("TurretRearArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double RearArmor => _primaryArmorKeys == null ? 0.0 : this.Armor.GetArmorValue(_primaryArmorKeys.Rear);

	    private Vector3D _gunPosition;
        public Vector3D GunPosition => _gunPosition;

	    public Turret(Database database)
            : base(database)
        {
            _availableGuns = new Dictionary<string, Gun>();
        }


        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "armor":
                    this.Armor = new Armor(((Tank)this.Owner).Nation.Database);
                    this.Armor.Deserialize(reader);
                    return true;

                case "primaryArmor":
                    reader.Read(out _primaryArmorKeys);
                    return true;

                case "armorHomogenization":
                    reader.Read(out _armorHomogenization);
                    return true;

                case "rotationSpeed":
                    reader.Read(out _rotationSpeed);
                    return true;

                case "turretRotatorHealth":
                    this.TurretRotator = new TurretRotator(this.Database);
                    this.TurretRotator.Deserialize(reader);
                    return true;

                case "circularVisionRadius":
                    reader.Read(out _circularVisionRadius);
                    return true;

                case "surveyingDeviceHealth":
                    this.SurveyingDevice = new SurveyingDevice(this.Database);
                    this.SurveyingDevice.Deserialize(reader);
                    return true;

                case "guns":
                    ((Tank)this.Owner).DeserializeModules(reader, _availableGuns, () => new Gun(this.Database), this);
                    return true;

                case "invisibilityFactor":
                    reader.Read(out _camouflageFactor);
                    return true;

                case "yawLimits":
                    reader.Read(out _horizontalTraverse);
                    return true;

                case "gunPosition":
                    reader.Read(out _gunPosition);
                    return true;

                case "tags":
                case "emblemSlots":
                case "showEmblemsOnGun":
                case "physicsShape":
                case "icon":
                case "stickers":
                case "camouflage":
                case "turretRotatorSound":
                case "turretRotatorSoundManual":
                case "turretRotatorSoundGear":
                case "ceilless":
                case "animateEmblemSlots":
                case "turretDetachmentEffects":
                case "AODecals": //?
                case "wwturretRotatorSoundManual":
                    return false;

                default:
                    if (base.DeserializeSection(name, reader))
                        return true;
                    else
                        return false;
            }

        }

        public override object Clone()
        {
            var turret = (Turret)base.Clone();
            turret._availableGuns = new Dictionary<string, Gun>();
            foreach (var gun in this._availableGuns)
                turret._availableGuns.Add(gun.Key, gun.Value);

            return turret;
        }

        protected override void OnSerializing(StreamingContext context)
        {
            base.OnSerializing(context);
            _availableGunsArray = new List<Gun>(_availableGuns.Values).ToArray();
        }

        protected override void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);
            _availableGuns = _availableGunsArray.ToDictionary(gun => gun.Key);
        }
    }
}
