using System.Collections.Generic;
namespace Smellyriver.TankInspector.Modeling
{
	internal interface IGun : IDamageableModule
    {
        double Accuracy { get; }
        double AimingTime { get; }
        double GunArmor { get; }
        Burst Burst { get; }
        Clip Clip { get; }
        HorizontalTraverse HorizontalTraverse { get; }
        int MaxAmmo { get; }
        double ReloadTime { get; }
        double RotationSpeed { get; }
        TurretShotDispersion ShotDispersion { get; }
        IEnumerable<IShell> Shots { get; }
        GunVerticalTraverse VerticalTraverse { get; }
        double CamouflageFactorAfterShot { get; }
    }
}
