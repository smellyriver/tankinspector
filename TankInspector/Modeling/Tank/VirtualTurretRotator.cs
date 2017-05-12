using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualTurretRotator : VirtualDamageableComponent, ITurretRotator
    {


        public override string Name => App.GetLocalizedString("TurretRotator");
    }
}
