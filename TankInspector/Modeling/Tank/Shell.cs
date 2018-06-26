using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Shell : Commodity, ICloneable, IShell
    {
        private string _name;
        
        public string Name => _name;

	    public ShellType Type { get; private set; }

	    private double _caliber;

        [Stat("ShellCaliberFullName", DataAnalysis.ComparisonMode.Plain)]
        public double Caliber => _caliber;

	    private double _explosionRadius;

        [Stat("ShellExplosionRadiusFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double ExplosionRadius => _explosionRadius;

	    private double _defaultPortion;
        
        public double DefaultPortion => _defaultPortion;

	    private double _speed;
        [Stat("ShellSpeedFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Speed => _speed;

	    private double _gravity;
        [Stat("ShellGravityFullName", DataAnalysis.ComparisonMode.Plain)]
        public double Gravity => _gravity;

	    private double _maxDistance;
        [Stat("ShellMaxDistanceFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double MaxDistance => _maxDistance;

	    private PiercingPower _piercingPower;
        [ExpandableStat]
        public PiercingPower PiercingPower => _piercingPower;

	    private ShellDamage _damage;
        [ExpandableStat]
        public ShellDamage Damage => _damage;

	    private double _piercingPowerLossFactorByDistance;
        [Stat("ShellPiercingPowerLossFactorByDistanceFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double PiercingPowerLossFactorByDistance => _piercingPowerLossFactorByDistance;

	    public Shell(Database database)
            : base(database)
        {

        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "userString":
                    reader.ReadLocalized(this.Database, out _name);
                    return true;
                case "kind":
                    this.Type = this.ParseShellType(reader.ReadString());
                    return true;
                case "caliber":
                    reader.Read(out _caliber);
                    return true;
                case "damage":
                    reader.Read(out _damage);
                    return true;
                case "explosionRadius":
                    reader.Read(out _explosionRadius);
                    return true;
                case "defaultPortion":
                    reader.Read(out _defaultPortion);
                    return true;
                case "speed":
                    reader.Read(out _speed);
                    return true;
                case "gravity":
                    reader.Read(out _gravity);
                    return true;
                case "piercingPower":
                    reader.Read(out _piercingPower);
                    return true;
                case "maxDistance":
                    reader.Read(out _maxDistance);
                    return true;
                case "piercingPowerLossFactorByDistance":
                    reader.Read(out _piercingPowerLossFactorByDistance);
                    return true;
                case "id":
                case "icon":
                case "isTracer":
                case "effects":
                    return false;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }


        private ShellType ParseShellType(string type)
        {
            switch (type)
            {
                case "ARMOR_PIERCING":
                    return ShellType.AP;
                case "ARMOR_PIERCING_CR":
                    return ShellType.APCR;
                case "HIGH_EXPLOSIVE":
                    return ShellType.HE;
                case "HOLLOW_CHARGE":
                    return ShellType.HEAT;
                case "ARMOR_PIERCING_HE":
                    return ShellType.APHE;
                case "HIGH_EXPLOSIVE_PREMIUM":
                    return ShellType.PremiumHE;
                case "SMOKE":
                    return ShellType.SMOKE;
                default:
                    throw new NotSupportedException();
            }
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"Shell: {this.Name}";
        }
    }
}
