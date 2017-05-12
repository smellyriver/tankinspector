using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkTank : VirtualTank
    {

        public const string DefaultNationKey = "benchmark";
        public const uint DefaultNationId = 15;
        public const uint DefaultTypeId = 2;

        public override uint TypeId => DefaultTypeId;

	    internal static BenchmarkTank Create(string key, uint id, IEnumerable<ITank> tanks)
        {
            var benchmark = new BenchmarkTank();
            benchmark.Key = key;
            benchmark.Id = id;

            BenchmarkTankObject.Initialize(benchmark, tanks);

            var premiumTanks = tanks.Where(t => t.CurrencyType == CurrencyType.Gold);
            benchmark.GoldPrice = premiumTanks.Count() == 0 ? 0 : premiumTanks.Average(t => t.Price);

            var regularTanks = tanks.Where(t => t.CurrencyType == CurrencyType.Credit);
            benchmark.CreditPrice = regularTanks.Count() == 0 ? 0 : regularTanks.Average(t => t.Price);

            const TankClass unknownTankClass = (TankClass)(-2);

            benchmark.Class = unknownTankClass;
            foreach (var tank in tanks)
            {
                if (benchmark.Class == unknownTankClass)
                    benchmark.Class = tank.Class;
                else if (benchmark.Class != TankClass.Mixed && benchmark.Class != tank.Class)
                    benchmark.Class = tank.Class;
            }

            benchmark.NationKey = DefaultNationKey;
            benchmark.NationId = DefaultNationId;

            // this value is not compared, dummy
            benchmark.BattleTiers = tanks.First().BattleTiers;
            var camouflages = tanks.Where(t => t.CamouflageValue != null).Select(t => t.CamouflageValue).ToArray();
            if (camouflages.Length > 0)
                benchmark.CamouflageValue = new CamouflageValue(camouflages.Average(c => c.Stationary), camouflages.Average(c => c.Moving), camouflages.Average(c => c.Firing));
            else
                benchmark.CamouflageValue = null;

            var invisibilities = tanks.Where(t => t.Invisibility != null).Select(t => t.Invisibility).ToArray();
            if (invisibilities.Length > 0)
                benchmark.Invisibility = new Invisibility(invisibilities.Average(i => i.Moving),
                                                          invisibilities.Average(i => i.Still),
                                                          invisibilities.Average(i => i.CamouflageBonus),
                                                          invisibilities.Average(i => i.CamouflageNetBonus),
                                                          invisibilities.Average(i => i.FirePenalty));
            else
                benchmark.Invisibility = null;

            benchmark.CrewExperienceFactor = tanks.Average(t => t.CrewExperienceFactor);

            var mmWeights = tanks.Where(t => t.MatchMakingWeight > 0).Select(t => t.MatchMakingWeight).ToArray();
            if (mmWeights.Length > 0)
                benchmark.MatchMakingWeight = mmWeights.Average();
            else
                benchmark.MatchMakingWeight = -1;

            benchmark.RepairCostFactor = tanks.Average(t => t.RepairCostFactor);
            benchmark.Chassis = BenchmarkChassis.Create(key + "Chassis", benchmark.ObjectKey, tanks.Select(t => t.AvailableChassis.Values.Last()));
            benchmark.Turret = BenchmarkTurret.Create(key + "Turret", benchmark.ObjectKey, tanks.Select(t => t.AvailableTurrets.Values.Last()));
            benchmark.Gun = benchmark.Turret.AvailableGuns.Values.Last();
            benchmark.Engine = BenchmarkEngine.Create(key + "Engine", benchmark.ObjectKey, tanks.Select(t => t.AvailableEngines.Values.Last()));
            benchmark.FuelTank = BenchmarkFuelTank.Create(key + "FuelTank", benchmark.ObjectKey, tanks.Select(t => t.AvailableFuelTanks.Values.Last()));
            benchmark.Radio = BenchmarkRadio.Create(key + "Radio", benchmark.ObjectKey, tanks.Select(t => t.AvailableRadios.Values.Last()));
            benchmark.Hull = BenchmarkHull.Create(key + "Hull", benchmark.ObjectKey, tanks.Select(t => t.Hull));

            benchmark.Crews.Add(new Crew { PrimaryRole = CrewRoleType.Commander });
            benchmark.Crews.Add(new Crew { PrimaryRole = CrewRoleType.Driver });
            benchmark.Crews.Add(new Crew { PrimaryRole = CrewRoleType.Gunner });
            benchmark.Crews.Add(new Crew { PrimaryRole = CrewRoleType.Loader });
            benchmark.Crews.Add(new Crew { PrimaryRole = CrewRoleType.Radioman });


            BenchmarkTank.WriteBenchmarkTankTags(benchmark);

            return benchmark;
        }

        private static void WriteBenchmarkTankTags(BenchmarkTank benchmark)
        {
            var tagList = new List<string>();
            switch (benchmark.Class)
            {
                case TankClass.HeavyTank:
                    tagList.Add("heavyTank");
                    break;
                case TankClass.MediumTank:
                    tagList.Add("mediumTank");
                    break;
                case TankClass.LightTank:
                    tagList.Add("lightTank");
                    break;
                case TankClass.TankDestroyer:
                    tagList.Add("AT-SPG");
                    break;
                case TankClass.SelfPropelledGun:
                    tagList.Add("SPG");
                    break;
            }

            benchmark.Tags = tagList.ToArray();
        }


        public override string ColonFullKey => $"{DefaultNationKey}:{this.Key}";

	    public double CreditPrice { get; internal set; }
        public double GoldPrice { get; internal set; }

        public void ApplyCreditPrice()
        {
            this.Price = (int)this.CreditPrice;
            this.CurrencyType = CurrencyType.Credit;
        }

        public void ApplyGoldPrice()
        {
            this.Price = (int)this.GoldPrice;
            this.CurrencyType = CurrencyType.Gold;
        }

    }
}
