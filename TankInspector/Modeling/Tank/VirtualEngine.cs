using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualEngine : VirtualDamageableModule, IEngine
    {


        public override ModuleType Type => ModuleType.Engine;

	    public double FireChance { get; internal set; }

        public FuelType FuelType { get; internal set; }

        public double HorsePower { get; internal set; }

    }
}
