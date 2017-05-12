using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkRadio : VirtualRadio
    {

        internal static BenchmarkRadio Create(string key, TankObjectKey ownerKey, IEnumerable<IRadio> radios)
        {
            var benchmark = new BenchmarkRadio();
            benchmark.Key = key;
            benchmark.OwnerKey = ownerKey;

            BenchmarkDamageableModule.Initialize(benchmark, radios);

            benchmark.Distance = radios.Average(r => r.Distance);

            return benchmark;
        }
    }
}
