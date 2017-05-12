using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualShell : VirtualCommodity, IShell
    {


        public string Name { get; internal set; }

        public double Caliber { get; internal set; }

        public ShellDamage Damage { get; internal set; }

        public double ExplosionRadius { get; internal set; }

        public double Gravity { get; internal set; }

        public double MaxDistance { get; internal set; }

        public PiercingPower PiercingPower { get; internal set; }

        public double PiercingPowerLossFactorByDistance { get; internal set; }

        public double Speed { get; internal set; }

        public ShellType Type { get; internal set; }

    }
}
