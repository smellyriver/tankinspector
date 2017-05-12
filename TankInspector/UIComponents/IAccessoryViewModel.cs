namespace Smellyriver.TankInspector.UIComponents
{
	internal interface IAccessoryViewModel : ISlotViewModel, IChangableComponent, IDetailedDataRelatedComponent
    {
        double GetWeight(double tankWeight);
        string Icon { get; }

        void NotifyIsEquippedChanged();
    }
}
