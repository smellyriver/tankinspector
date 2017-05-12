using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class TurretRotator : DamageableComponent, ITurretRotator
    {
        public override string Name => App.GetLocalizedString("TurretRotator");

	    public TurretRotator(Database database)
            : base(database)
        {

        }
    }
}
