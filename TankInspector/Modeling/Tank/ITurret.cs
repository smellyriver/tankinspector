using System.Collections.Generic;
namespace Smellyriver.TankInspector.Modeling
{
	internal interface ITurret : IDamageableModule
    {
        double ArmorHomogenization { get; }
        Dictionary<string, IGun> AvailableGuns { get; }
        double CamouflageFactor { get; }
        double CircularVisionRadius { get; }
        double FrontalArmor { get; }
        HorizontalTraverse HorizontalTraverse { get; }
        bool IsArmorDefined { get; }
        double RearArmor { get; }
        double RotationSpeed { get; }
        double SideArmor { get; }
        ISurveyingDevice SurveyingDevice { get; }
        ITurretRotator TurretRotator { get; }
    }
}
