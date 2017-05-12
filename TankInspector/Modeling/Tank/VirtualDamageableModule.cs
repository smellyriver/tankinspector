using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class VirtualDamageableModule : VirtualModule, IDamageableModule
    {
        
        public double MaxHealth { get; internal set; }

        public double MaxRegenHealth { get; internal set; }

        public double RepairCostFactor { get; internal set; }

        public double BulkHealthFactor { get; internal set; }
    }
}
