using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkFuelTank : VirtualFuelTank
    {
        internal static BenchmarkFuelTank Create(string key, TankObjectKey ownerKey, IEnumerable<IFuelTank> fuelTanks)
        {
            var benchmark = new BenchmarkFuelTank();
            benchmark.Key = key;
            benchmark.OwnerKey = ownerKey;

            BenchmarkDamageableModule.Initialize(benchmark, fuelTanks);
            return benchmark;
        }
    }
}
