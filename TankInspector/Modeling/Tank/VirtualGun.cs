using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualGun : VirtualDamageableModule, IGun
    {
        internal static IGun CreateBenchmarkGun(string key, TankObjectKey ownerKey, IEnumerable<IGun> guns)
        {
            var benchmark = new VirtualGun();
            benchmark.Key = key;
            benchmark.OwnerKey = ownerKey;
            BenchmarkDamageableModule.Initialize(benchmark, guns);
            benchmark.Accuracy = guns.Average(g => g.Accuracy);
            benchmark.AimingTime = guns.Average(g => g.AimingTime);
            benchmark.GunArmor = guns.Average(g => g.GunArmor);

            var burstGuns = guns.Where(g => g.Burst != null);
            if (burstGuns.Count() == 0)
                benchmark.Burst = null;
            else
                benchmark.Burst = new Burst((int)burstGuns.Average(g => g.Burst.Count), burstGuns.Average(g => g.Burst.Rate));

            benchmark.CamouflageFactorAfterShot = guns.Average(g => g.CamouflageFactorAfterShot);

            var clipGuns = guns.Where(g => g.Clip != null);
            if (clipGuns.Count() == 0)
                benchmark.Clip = null;
            else
                benchmark.Clip = new Clip((int)clipGuns.Average(g => g.Clip.Count), clipGuns.Average(g => g.Clip.Rate));

            benchmark.HorizontalTraverse = new HorizontalTraverse(guns.Median(g => g.HorizontalTraverse.Left), guns.Median(g => g.HorizontalTraverse.Right));
            benchmark.MaxAmmo = (int)guns.Average(g => g.MaxAmmo);
            benchmark.ReloadTime = guns.Average(g => g.ReloadTime);
            benchmark.RotationSpeed = guns.Average(g => g.RotationSpeed);
            benchmark.ShotDispersion = new TurretShotDispersion(guns.Average(g => g.ShotDispersion.AfterShot), guns.Average(g => g.ShotDispersion.TurretRotation), guns.Average(g => g.ShotDispersion.GunDamaged));

            var shellGroups = new List<KeyValuePair<IGun, IShell>>[4];
            for (int i = 0; i < 4; ++i)
                shellGroups[i] = new List<KeyValuePair<IGun, IShell>>();

            foreach (var gun in guns)
                foreach (var shell in gun.Shots)
                {
                    var kineticFlag = shell.Type.IsKineticShellType() ? 2 : 0;
                    var premiumFlag = shell.CurrencyType == CurrencyType.Gold ? 1 : 0;
                    shellGroups[kineticFlag | premiumFlag].Add(new KeyValuePair<IGun, IShell>(gun, shell));
                }

            int shellIndex = 0;
            var shots = new List<IShell>();
            foreach (var shellGroup in shellGroups.Where(g => g.Count > 0))
            {
                var shellKey = $"{key}Shell{shellIndex++}";
                var benchmarkShell = BenchmarkShell.Create(shellKey, shellGroup.Select(g => g.Value));
                benchmarkShell.DamagePerMinute = shellGroup.Average(g => VirtualGun.CalculateDpm(g.Key, g.Value));
                shots.Add(benchmarkShell);
            }

            benchmark.Shots = shots.ToArray();

            benchmark.VerticalTraverse = new GunVerticalTraverse();
            benchmark.VerticalTraverse.DefaultPitchLimits = new PitchLimits(guns.Average(g => g.VerticalTraverse.Front.Elevation), guns.Average(g => g.VerticalTraverse.Front.Depression));
            return benchmark;
        }

        private static double CalculateDpm(IGun gun, IShell shell)
        {
            var clipCount = gun.Clip == null ? 1 : gun.Clip.Count;
            return (60.0 / gun.ReloadTime) * clipCount * shell.Damage.Armor;
        }

        public override ModuleType Type => ModuleType.Gun;

	    public double Accuracy { get; internal set; }

        public double AimingTime { get; internal set; }

        public double GunArmor { get; internal set; }

        public Burst Burst { get; internal set; }

        public double CamouflageFactorAfterShot { get; internal set; }

        public Clip Clip { get; internal set; }

        public HorizontalTraverse HorizontalTraverse { get; internal set; }

        public int MaxAmmo { get; internal set; }

        public double ReloadTime { get; internal set; }

        public double RotationSpeed { get; internal set; }

        public TurretShotDispersion ShotDispersion { get; internal set; }

        public IEnumerable<IShell> Shots { get; internal set; }

        public GunVerticalTraverse VerticalTraverse { get; internal set; }


    }
}
