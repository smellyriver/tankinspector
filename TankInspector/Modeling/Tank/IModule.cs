namespace Smellyriver.TankInspector.Modeling
{
	internal interface IModule : ITankObject
    {
        ModuleType Type { get; }
        double Weight { get; }
    }
}
