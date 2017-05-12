using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualRadio : VirtualDamageableModule, IRadio
    {

        public override ModuleType Type => ModuleType.Radio;

	    public double Distance { get; internal set; }

    }
}
