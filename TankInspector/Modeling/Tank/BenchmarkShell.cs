using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkShell : VirtualShell
    {

        internal static BenchmarkShell Create(string key, IEnumerable<IShell> shells)
        {
            var benchmark = new BenchmarkShell();
            benchmark.Key = key;

            BenchmarkCommodity.Initialize(benchmark, shells);

            benchmark.Caliber = shells.Median(s => s.Caliber);
            benchmark.Damage = new ShellDamage(shells.Average(s => s.Damage.Armor), shells.Average(s => s.Damage.Devices));
            benchmark.ExplosionRadius = shells.Average(s => s.ExplosionRadius);
            benchmark.Gravity = shells.Average(s => s.Gravity);
            benchmark.MaxDistance = shells.Average(s => s.MaxDistance);
            benchmark.PiercingPower = new PiercingPower(shells.Average(s => s.PiercingPower.P100), shells.Average(s => s.PiercingPower.P400));
            benchmark.PiercingPowerLossFactorByDistance = shells.Average(s => s.PiercingPowerLossFactorByDistance);
            benchmark.Speed = shells.Average(s => s.Speed);
            benchmark.Type = shells.Majority(s => s.Type);

            return benchmark;
        }

        public double DamagePerMinute { get; internal set; }
    }
}
