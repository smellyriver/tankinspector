using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class AmmoBay : DamageableComponent, IAmmoBay
    {
        public override string Name => App.GetLocalizedString("AmmoBay");

	    public AmmoBay(Database database)
            : base(database)
        {

        }
    }
}
