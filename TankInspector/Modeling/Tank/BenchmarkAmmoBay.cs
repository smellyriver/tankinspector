using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkAmmoBay:VirtualAmmoBay
    {
        internal static BenchmarkAmmoBay Create(IEnumerable<IAmmoBay> enumerable)
        {
            var benchmark = new BenchmarkAmmoBay();
            BenchmarkDamageableComponent.Initialize(benchmark, enumerable);
            return benchmark;
        }

    }
}
