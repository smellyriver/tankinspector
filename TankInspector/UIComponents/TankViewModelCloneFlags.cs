namespace Smellyriver.TankInspector.UIComponents
{
	internal enum TankViewModelCloneFlags
    {
        AnyModule = 0x100,
        Gun = 0x101,
        Turret = 0x102,
        Hull = 0x104,
        Chassis = 0x108,
        Engine = 0x110,
        Radio = 0x111,
        FuelTank = 0x112,
        AllModules = Gun | Turret | Hull | Chassis | Engine | Radio | FuelTank,
        AnyModificationEffect = 0x1000,
        AnyAccessory = 0x1100,
        Equipments = 0x1110,
        Consumables = 0x1120,
        AnyCrew = 0x1200,
        Commander = 0x1201,
        Driver = 0x1202,
        Gunner = 0x1204,
        Loader = 0x1208,
        Radioman = 0x1210,
        AllCrews = Commander | Driver | Gunner | Loader | Radioman,
        All = AllModules | Equipments | Consumables | AllCrews
    }
}
