using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkEngine : VirtualEngine
    {
        internal static BenchmarkEngine Create(string key, TankObjectKey ownerKey, IEnumerable<IEngine> engines)
        {
            var benchmark = new BenchmarkEngine();
            benchmark.Key = key;
            benchmark.OwnerKey = ownerKey;

            BenchmarkDamageableModule.Initialize(benchmark, engines);
            benchmark.FireChance = engines.Average(e => e.FireChance);
            benchmark.FuelType = engines.Majority(e => e.FuelType);
            benchmark.HorsePower = engines.Average(e => e.HorsePower);
            return benchmark;
        }
    }
}
