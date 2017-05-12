using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkTurret : VirtualTurret
    {

        internal static BenchmarkTurret Create(string key, TankObjectKey ownerKey, IEnumerable<ITurret> turrets)
        {
            var benchmark = new BenchmarkTurret();
            benchmark.Key = key;
            benchmark.OwnerKey = ownerKey;
            BenchmarkDamageableModule.Initialize(benchmark, turrets);

            benchmark.ArmorHomogenization = turrets.Average(t => t.ArmorHomogenization);
            benchmark.CamouflageFactor = turrets.Average(t => t.CamouflageFactor);
            benchmark.CircularVisionRadius = (int)turrets.Average(t => t.CircularVisionRadius);

            if (turrets.All(t => !t.IsArmorDefined))
                benchmark.IsArmorDefined = false;
            else
            {
                benchmark.IsArmorDefined = true;
                benchmark.FrontalArmor = turrets.Where(t => t.IsArmorDefined).Average(t => t.FrontalArmor);
                benchmark.SideArmor = turrets.Where(t => t.IsArmorDefined).Average(t => t.SideArmor);
                benchmark.RearArmor = turrets.Where(t => t.IsArmorDefined).Average(t => t.RearArmor);
            }

            benchmark.Gun = VirtualGun.CreateBenchmarkGun(ownerKey + "Gun", benchmark.ObjectKey, turrets.Select(t => BenchmarkTurret.GetEliteGun(t)));

            var horizontallyLimitedTurrets = turrets.Where(t => t.HorizontalTraverse != null);
            if (horizontallyLimitedTurrets.Count() == 0)
                benchmark.HorizontalTraverse = null;
            else
                benchmark.HorizontalTraverse = new HorizontalTraverse(horizontallyLimitedTurrets.Average(t => t.HorizontalTraverse.Left), horizontallyLimitedTurrets.Average(t => t.HorizontalTraverse.Right));

            benchmark.RotationSpeed = turrets.Average(t => t.RotationSpeed);
            benchmark.SurveyingDevice = BenchmarkSurveyingDevice.Create(turrets.Select(t => t.SurveyingDevice));
            benchmark.TurretRotator = BenchmarkTurretRotator.Create(turrets.Select(t => t.TurretRotator));

            return benchmark;
        }

        private static IGun GetEliteGun(ITurret turret)
        {
            var maxTier = turret.AvailableGuns.Values.Max(gun => gun.Tier);
            return turret.AvailableGuns.Values.Where(gun => gun.Tier == maxTier).LastOrDefault();
        }
    }
}
