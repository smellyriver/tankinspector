using Smellyriver.TankInspector.DataAnalysis;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class TankViewModelBase
	{


		public static readonly DataItemDescriptor<TankViewModelBase> HealthDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> EquipmentWeightDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> WeightDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> RepairCostDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> MatchmakingWeightDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> BattleTiersDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> PriceDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> StationaryCamouflageDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> MovingCamouflageDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> FiringStationaryCamouflageDescriptor;
		public static readonly DataItemDescriptor<TankViewModelBase> FiringMovingCamouflageDescriptor;

		static TankViewModelBase()
		{
			HealthDescriptor = new DataItemDescriptor<TankViewModelBase>("TankHealthFullName", tank => tank, tank => tank.Health,
				"UnitHealthPoints", "TankHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);
			WeightDescriptor = new DataItemDescriptor<TankViewModelBase>("TankWeightFullName", tank => tank, tank => tank.Weight,
				"UnitKilograms", "TankWeightDescription", "{0:#,0}", ComparisonMode.Plain);
			EquipmentWeightDescriptor = new DataItemDescriptor<TankViewModelBase>("TankEquipmentWeightFullName", tank => tank,
				tank => tank.EquipmentWeight, "UnitKilograms", "TankEquipmentWeightDescription", "{0:#,0}", ComparisonMode.Plain);
			RepairCostDescriptor = new DataItemDescriptor<TankViewModelBase>("TankRepairCostFullName", tank => tank,
				tank => tank.RepairCost, "UnitCredits", "TankRepairCostDescription", "{0:#,0.#}", ComparisonMode.LowerBetter);
			MatchmakingWeightDescriptor = new DataItemDescriptor<TankViewModelBase>("TankMatchmakingWeightFullName",
				tank => tank, tank => tank.Tank.MatchMakingWeight, null, "TankMatchmakingWeightDescription", "{0:#,0.#}",
				ComparisonMode.LowerBetter);
			BattleTiersDescriptor = new DataItemDescriptor<TankViewModelBase>("TankBattleTiersFullName", tank => tank,
				tank => tank.BattleTiers, null, "TankBattleTiersDescription", "{0}", ComparisonMode.NotComparable, isDecimal: false,
				benchmarkRatio: 0.02);
			PriceDescriptor = new DataItemDescriptor<TankViewModelBase>("TankPriceFullName", tank => tank, tank => tank.Price,
				null, "TankPriceDescription", "{0}", ComparisonMode.LowerBetter);
			StationaryCamouflageDescriptor = new DataItemDescriptor<TankViewModelBase>("TankStationaryCamouflageFullName",
				tank => tank, tank => tank.StationaryCamoflagueValue, null, "TankStationaryCamouflageDescription", "{0}",
				ComparisonMode.HigherBetter, isDecimal: false);
			MovingCamouflageDescriptor = new DataItemDescriptor<TankViewModelBase>("TankMovingCamouflageFullName", tank => tank,
				tank => tank.MovingCamoflagueValue, null, "TankMovingCamouflageDescription", "{0}", ComparisonMode.HigherBetter,
				isDecimal: false);
			FiringStationaryCamouflageDescriptor = new DataItemDescriptor<TankViewModelBase>(
				"TankFiringStationaryCamouflageFullName", tank => tank, tank => tank.FiringStationaryCamoflagueValue, null,
				"TankFiringStationaryCamouflageDescription", "{0}", ComparisonMode.HigherBetter, isDecimal: false);
			FiringMovingCamouflageDescriptor = new DataItemDescriptor<TankViewModelBase>("TankFiringMovingCamouflageFullName",
				tank => tank, tank => tank.FiringMovingCamoflagueValue, null, "TankFiringMovingCamouflageDescription", "{0}",
				ComparisonMode.HigherBetter, isDecimal: false);
		}
	}


}
