using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class VirtualDamageableComponent : IDamageableComponent
    {

        public abstract string Name { get; }

        public double MaxHealth { get; internal set; }

        public double MaxRegenHealth { get; internal set; }

        public double RepairCostFactor { get; internal set; }

        public double BulkHealthFactor { get; internal set; }
    }
}
