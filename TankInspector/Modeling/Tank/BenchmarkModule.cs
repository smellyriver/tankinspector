using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal static class BenchmarkModule
    {
        internal static void Initialize(VirtualModule target, IEnumerable<IModule> modules)
        {
            BenchmarkTankObject.Initialize(target, modules);
            target.Weight = modules.Average(m => m.Weight);
        }
    }
}
