namespace Smellyriver.TankInspector.Modeling
{
	internal interface IHull : IDamageableModule
    {
        IAmmoBay AmmoBay { get; }
        double FrontalArmor { get; }
        double RearArmor { get; }
        double SideArmor { get; }
    }
}
