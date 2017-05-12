namespace Smellyriver.TankInspector.UIComponents
{
	internal interface ISlotViewModel
    {
        int Tier { get; }
        string Name { get; }
        string Description { get; }
        bool IsLoadCapable { get; }
    }
}
