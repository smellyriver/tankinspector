using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualHull : VirtualDamageableModule, IHull
    {

        public override ModuleType Type => ModuleType.Hull;

	    public IAmmoBay AmmoBay { get; internal set; }

        public double FrontalArmor { get; internal set; }

        public double RearArmor { get; internal set; }

        public double SideArmor { get; internal set; }

        
    }
}
