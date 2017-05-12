using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class EmptyEquipmentViewModel : EmptyAccessoryViewModel, IEquipmentViewModel
    {
        public override IEnumerable Replacements => Owner.AvailableEquipments.Values;


	    IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }

        public override string Name => App.GetLocalizedString("EmptyEquipmentSlot");

	    public override string Description => App.GetLocalizedString("EmptyEquipmentSlotDescription");

	    public EmptyEquipmentViewModel(CommandBindingCollection commandBindings, TankViewModelBase owner)
            : base(commandBindings, owner)
        {

        }



    }
}
