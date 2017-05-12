using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualFuelTank : VirtualDamageableModule, IFuelTank
    {


        public override ModuleType Type => ModuleType.FuelTank;
    }
}
