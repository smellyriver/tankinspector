using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualAmmoBay : VirtualDamageableComponent, IAmmoBay
    {
        public override string Name => App.GetLocalizedString("AmmoBay");
    }
}
