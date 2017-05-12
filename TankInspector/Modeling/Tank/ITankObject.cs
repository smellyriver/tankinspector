namespace Smellyriver.TankInspector.Modeling
{
	internal interface ITankObject : ICommodity
    {
        string Name { get; }
        TankObjectKey ObjectKey { get; }
        TankObjectType ObjectType { get; }
        string ShortName { get; }
        int Tier { get; }

    }
}
