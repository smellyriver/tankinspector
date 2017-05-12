using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Chassis : DamageableModule, IHasArmor, IChassis
    {

        public override ModuleType Type => ModuleType.Chasis;

	    public Armor Armor { get; private set; }

	    [Stat("ChassisArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double TrackArmor => this.Armor.ArmorGroups["leftTrack"].Value;

	    private double _maxClimbAngle;
        public double MaxClimbAngle => _maxClimbAngle;

	    private double _maxLoad;
        [Stat("ChassisMaxLoadFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double MaxLoad => _maxLoad;

	    private TerrainResistance _terrainResistance;
        [ExpandableStat]
        public TerrainResistance TerrainResistance => _terrainResistance;

	    private double _brakeForce;
        [Stat("ChassisBrakeForceFullName")]
        public double BrakeForce => _brakeForce;

	    private bool _canTraverse;
        [Stat("ChassisBrakeForceFullName", DataAnalysis.ComparisonMode.NotComparable)]
        public bool CanTraverse => _canTraverse;

	    private double _rotationSpeed;
        [Stat("ChassisRotationSpeedFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double RotationSpeed => _rotationSpeed;

	    private ChassisShotDispersion _shotDispersion;
        [ExpandableStat]
        public ChassisShotDispersion ShotDispersion => _shotDispersion;

	    private TrackMaterial _trackMaterial;
        public TrackMaterial TrackMaterial => _trackMaterial;

	    public WheelInfo Wheels { get; }

	    private Vector3D _hullPosition;
        public Vector3D HullPosition => _hullPosition;

	    [ExpandableStat]
        public SpeedLimits SpeedLimits { get; internal set; }



        public Chassis(Database database)
            : base(database)
        {
            this.Wheels = new WheelInfo();
        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "armor":
                    this.Armor = new Armor(((Tank)this.Owner).Nation.Database);
                    this.Armor.Deserialize(reader);
                    return true;

                case "maxClimbAngle":
                    reader.Read(out _maxClimbAngle);
                    return true;

                case "maxLoad":
                    reader.Read(out _maxLoad);
                    return true;

                case "terrainResistance":
                    reader.Read(out _terrainResistance);
                    return true;

                case "brakeForce":
                    reader.Read(out _brakeForce);
                    return true;

                case "rotationIsAroundCenter":
                    reader.Read(out _canTraverse);
                    return true;


                case "rotationSpeed":
                    reader.Read(out _rotationSpeed);
                    return true;

                case "shotDispersionFactors":
                    reader.Read(out _shotDispersion);
                    return true;

                case "tracks":
                    reader.Read(out _trackMaterial);
                    return true;

                case "wheels":
                    this.Wheels.Deserialize(reader);
                    return true;

                case "drivingWheels":
                    var drivingWheels = reader.ReadString();
                    foreach (var drivingWheel in drivingWheels.Split(' '))
                        this.Wheels.DrivingWheelNames.Add(drivingWheel);

                    return true;

                case "hullPosition":
                    reader.Read(out _hullPosition);
                    return true;

                case "tags":
                case "traces":
                case "effects":
                case "sound":
                case "topRightCarryingPoint":
                case "navmeshGirth":
                case "icon":
                    return false;

                default:
                    if (base.DeserializeSection(name, reader))
                        return true;
                    else
                        return false;
            }

        }


    }
}
