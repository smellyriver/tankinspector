using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkHull : VirtualHull
    {
        internal static BenchmarkHull Create(string key, TankObjectKey ownerKey, IEnumerable<IHull> hulls)
        {
            var benchmark = new BenchmarkHull();
            benchmark.Key = key;
            benchmark.OwnerKey = ownerKey;

            BenchmarkDamageableModule.Initialize(benchmark, hulls);
            benchmark.AmmoBay = BenchmarkAmmoBay.Create(hulls.Select(h => h.AmmoBay));
            benchmark.FrontalArmor = hulls.Average(h => h.FrontalArmor);
            benchmark.RearArmor = hulls.Average(h => h.RearArmor);
            benchmark.SideArmor = hulls.Average(h => h.SideArmor);

            return benchmark;
        }
    }
}
