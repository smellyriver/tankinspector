using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class EmptyAccessoryViewModel : AccessoryViewModelBase
    {
        public override string Icon => @"gui/maps/icons/artefact/empty.png";

	    public override int Tier => -1;

	    public override bool IsLoadCapable => true;

	    public override bool IsElite => false;

	    public override bool IsEquipped => false;


	    public override ICommand EquipCommand => null;

	    public override DetailedDataRelatedComponentType ComponentType => DetailedDataRelatedComponentType.None;

	    public EmptyAccessoryViewModel(CommandBindingCollection commandBindings, TankViewModelBase owner)
            :base(commandBindings, owner)
        {
        }

        public virtual void NotifyIsEquippedChanged()
        {
            
        }

        public override double GetWeight(double tankWeight)
        {
            return 0;
        }
    }
}
