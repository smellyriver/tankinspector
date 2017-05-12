using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class EmptyConsumableViewModel : EmptyAccessoryViewModel, IConsumableViewModel
    {
        public override IEnumerable Replacements => Owner.AvailableConsumables.Values;

	    public IEnumerable<string> Tags
        {
            get { yield break; }
        }

        public IEnumerable<string> IncompatibleTags
        {
            get { yield break; }
        }
        IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }

        public override string Name => App.GetLocalizedString("EmptyConsumableSlot");

	    public override string Description => App.GetLocalizedString("EmptyConsumableSlotDescription");

	    public override ICommand EquipCommand => null;

	    public EmptyConsumableViewModel(CommandBindingCollection commandBindings, TankViewModelBase owner)
            : base(commandBindings, owner)
        {

        }

    }
}
