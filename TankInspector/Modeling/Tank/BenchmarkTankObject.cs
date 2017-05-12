using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal static class BenchmarkTankObject
    {

        internal static void Initialize(VirtualTankObject target, IEnumerable<ITankObject> objects)
        {
            BenchmarkCommodity.Initialize(target, objects);
            target.Tier = (int)objects.Average(m => m.Tier);
        }
    }
}
