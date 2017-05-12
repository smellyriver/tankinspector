using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal static class BenchmarkCommodity
    {
        internal static void Initialize(VirtualCommodity target, IEnumerable<ICommodity> commodities)
        {
            target.Price = (int)commodities.Average(m => m.Price);
            target.CurrencyType = commodities.Majority(m => m.CurrencyType);
        }
    }
}
