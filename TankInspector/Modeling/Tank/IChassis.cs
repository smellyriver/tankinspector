namespace Smellyriver.TankInspector.Modeling
{
	internal interface IChassis : IDamageableModule
    {
        double TrackArmor { get; }
        double BrakeForce { get; }
        bool CanTraverse { get; }
        double MaxClimbAngle { get; }
        double MaxLoad { get; }
        double RotationSpeed { get; }
        ChassisShotDispersion ShotDispersion { get; }
        SpeedLimits SpeedLimits { get; }
        TerrainResistance TerrainResistance { get; }
    }
}
