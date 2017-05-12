namespace Smellyriver.TankInspector.Modeling
{
	internal interface ITankInfo
    {
        string Key { get; }
        string Name { get; }
        string ShortName { get; }
        string IconKey { get; }
        string ColonFullKey { get; }
        TankClass Class { get; }
        int Tier { get; }
    }
}
