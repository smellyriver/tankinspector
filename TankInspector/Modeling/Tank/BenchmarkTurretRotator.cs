using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkTurretRotator : VirtualTurretRotator
    {
        internal static BenchmarkTurretRotator Create(IEnumerable<ITurretRotator> enumerable)
        {
            var benchmark = new BenchmarkTurretRotator();
            BenchmarkDamageableComponent.Initialize(benchmark, enumerable);
            return benchmark;
        }
    }
}
