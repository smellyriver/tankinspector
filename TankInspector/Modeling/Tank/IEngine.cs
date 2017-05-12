namespace Smellyriver.TankInspector.Modeling
{
	internal interface IEngine : IDamageableModule
    {
        double FireChance { get; }
        FuelType FuelType { get; }
        double HorsePower { get; }
    }
}
