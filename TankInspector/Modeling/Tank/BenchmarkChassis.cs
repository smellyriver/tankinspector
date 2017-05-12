using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkChassis : VirtualChassis
    {
        internal static BenchmarkChassis Create(string key, TankObjectKey ownerKey, IEnumerable<IChassis> chassis)
        {
            var benchmark = new BenchmarkChassis();
            benchmark.Key = key;
            benchmark.OwnerKey = ownerKey;

            BenchmarkDamageableModule.Initialize(benchmark, chassis);
            benchmark.BrakeForce = chassis.Average(c => c.BrakeForce);
            benchmark.CanTraverse = chassis.Majority(c => c.CanTraverse);
            benchmark.MaxClimbAngle = chassis.Average(c => c.MaxClimbAngle);
            benchmark.MaxLoad = chassis.Average(c => c.MaxLoad);
            benchmark.RotationSpeed = chassis.Average(c => c.RotationSpeed);
            benchmark.ShotDispersion = new ChassisShotDispersion(chassis.Average(c => c.ShotDispersion.Movement), chassis.Average(c => c.ShotDispersion.Rotation));
            benchmark.SpeedLimits = new SpeedLimits(chassis.Average(c => c.SpeedLimits.Forward), chassis.Average(c => c.SpeedLimits.Backward));
            benchmark.TerrainResistance = new TerrainResistance(chassis.Average(c => c.TerrainResistance.HardTerrain), chassis.Average(c => c.TerrainResistance.MediumTerrain), chassis.Average(c => c.TerrainResistance.SoftTerrain));
            benchmark.TrackArmor = chassis.Average(c => c.TrackArmor);
            return benchmark;
        }
    }
}
