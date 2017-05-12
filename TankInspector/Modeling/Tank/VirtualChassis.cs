using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualChassis : VirtualDamageableModule, IChassis
    {

        public override ModuleType Type => ModuleType.Chasis;


	    public double TrackArmor { get; internal set; }

        public double BrakeForce { get; internal set; }

        public bool CanTraverse { get; internal set; }

        public double MaxClimbAngle { get; internal set; }

        public double MaxLoad { get; internal set; }

        public double RotationSpeed { get; internal set; }

        public ChassisShotDispersion ShotDispersion { get; internal set; }

        public SpeedLimits SpeedLimits { get; internal set; }

        public TerrainResistance TerrainResistance { get; internal set; }




    }
}
