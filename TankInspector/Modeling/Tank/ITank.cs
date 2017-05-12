using System.Collections.Generic;
namespace Smellyriver.TankInspector.Modeling
{
	internal interface ITank : ITankObject, ITankInfo
    {
        Dictionary<string, IChassis> AvailableChassis { get; }
        Dictionary<string, IEngine> AvailableEngines { get; }
        Dictionary<string, IFuelTank> AvailableFuelTanks { get; }
        Dictionary<string, IGun> AvailableGuns { get; }
        Dictionary<string, IRadio> AvailableRadios { get; }
        Dictionary<string, ITurret> AvailableTurrets { get; }
        Dictionary<string, Equipment> AvailableEquipments { get; }
        Dictionary<string, Consumable> AvailableConsumables { get; }
        BattleTierSpan BattleTiers { get; }
        CamouflageValue CamouflageValue { get; }    // pre 9.15
        Invisibility Invisibility { get; }  // post 9.15
        double CrewExperienceFactor { get; }
        List<Crew> Crews { get; }
        string Description { get; }
        IHull Hull { get; }
        double MatchMakingWeight { get; }
        double RepairCostFactor { get; }

        string NationKey { get; }

        string[] Tags { get; }

        Database Database { get; }

        uint Id { get; }
        uint TypeId { get; }
        uint NationId { get;}

    }
}
