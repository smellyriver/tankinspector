using System;
using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Gun : DamageableModule, IHasArmor, IGun
    {
        public override ModuleType Type => ModuleType.Gun;

	    private Armor _armorSchema;

        [Stat("GunArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double GunArmor => _armorSchema.ArmorGroups["gun"].Value;

	    Armor IHasArmor.Armor => _armorSchema;

	    private double _accuracy;

        [Stat("GunAccuracyFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double Accuracy => _accuracy;

	    private double _reloadTime;
        [Stat("GunReloadTimeFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double ReloadTime => _reloadTime;

	    private double _aimingTime;

        [Stat("GunAimingTimeFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double AimingTime => _aimingTime;

	    private double _camouflageFactorAfterShot;

        [Stat("CamouflageFactorAfterShot", DataAnalysis.ComparisonMode.HigherBetter)]
        public double CamouflageFactorAfterShot => _camouflageFactorAfterShot;

	    private TurretShotDispersion _shotDispersion;

        [ExpandableStat]
        public TurretShotDispersion ShotDispersion => _shotDispersion;

	    [ExpandableStat]
        public GunVerticalTraverse VerticalTraverse { get; private set; }

        private HorizontalTraverse _horizontalTraverse;

        [ExpandableStat]
        public HorizontalTraverse HorizontalTraverse => _horizontalTraverse;

	    private double _rotationSpeed;

        [Stat("GunRotationSpeedFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double RotationSpeed => _rotationSpeed;

	    private int _maxAmmo;
        [Stat("GunMaxAmmoFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public int MaxAmmo => _maxAmmo;


	    private Clip _clip;
        [ExpandableStat]
        public Clip Clip => _clip;

	    private Burst _burst;
        [ExpandableStat]
        public Burst Burst => _burst;


	    [StatGroup("Shells")]
        public Dictionary<string, Shell> Shots { get; private set; }

	    IEnumerable<IShell> IGun.Shots => this.Shots.Values;

	    public Gun(Database database)
            : base(database)
        {
            this.VerticalTraverse = new GunVerticalTraverse();
        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "pitchLimits":
                    PitchLimits pitchLimits;
                    reader.Read(out pitchLimits);
                    if ((pitchLimits.ElevationData == null || pitchLimits.DepressionData == null)
                        && this.VerticalTraverse.DefaultPitchLimits != null)
                        return true;
                    this.VerticalTraverse.DefaultPitchLimits = pitchLimits;
                    return true;
                case "turretYawLimits":
                    reader.Read(out _horizontalTraverse);
                    return true;
                case "rotationSpeed":
                    reader.Read(out _rotationSpeed);
                    return true;
                case "reloadTime":
                    reader.Read(out _reloadTime);
                    return true;

                case "maxAmmo":
                    reader.Read(out _maxAmmo);
                    return true;

                case "aimingTime":
                    reader.Read(out _aimingTime);
                    return true;

                case "shotDispersionRadius":
                    reader.Read(out _accuracy);
                    return true;

                case "shotDispersionFactors":
                    reader.Read(out _shotDispersion);
                    return true;

                case "shots":
                    this.Shots = new Dictionary<string, Shell>();

					// in v0.9.18, UK gun data can contain random text node
	                while (reader.NodeType == XmlNodeType.Text)
		                reader.Skip();

                    while (reader.IsStartElement())
                    {
                        var shotName = reader.Name;
						if (!this.Nation.Shells.TryGetValue(shotName, out Shell shell))
							shell = new Shell(this.Database);
						else
							shell = (Shell)shell.Clone();

						shell.Deserialize(reader);
                        this.Shots.Add(shell.Key, shell);
                    }

                    return true;

                case "clip":
                    reader.Read(out _clip);
                    return true;

                case "burst":
                    reader.Read(out _burst);
                    return true;

                case "armor":
                    _armorSchema = new Armor(((Tank)((Turret)this.Owner).Owner).Nation.Database);
                    _armorSchema.Deserialize(reader);
                    return true;

                case "invisibilityFactorAtShot":
                    reader.Read(out _camouflageFactorAfterShot);
                    return true;

                case "extraPitchLimits":
                    ExtraPitchLimits extraPitchLimits;
                    reader.Read(out extraPitchLimits);
                    this.VerticalTraverse.ExtraPitchLimits = extraPitchLimits;
                    return true;

                case "impulse":
                case "recoil":
                case "effects":
                case "tags":
                case "groundWave":
                case "camouflage":
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
            var gun = (Gun)base.Clone();
            gun.Shots = new Dictionary<string, Shell>();
            foreach (var shell in this.Shots)
                gun.Shots.Add(shell.Key, shell.Value);

            gun.VerticalTraverse = this.VerticalTraverse.Clone();

            return gun;
        }


    }
}
