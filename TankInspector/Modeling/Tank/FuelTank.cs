using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class FuelTank : DamageableModule, IFuelTank
    {
        public override ModuleType Type => ModuleType.FuelTank;

	    public FuelTank(Database database)
            : base(database)
        {

        }
    }
}
