using Smellyriver.Wpf.Input;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class RemoveEquipmentViewModel : EmptyAccessoryViewModel, IEquipmentViewModel
    {

        public override string Name => App.GetLocalizedString("RemoveEquipment");

	    public override string Description => App.GetLocalizedString("RemoveEquipmentDescription");

	    public override ICommand EquipCommand { get; }

	    public override IEnumerable Replacements => Owner.EquipmentReplacements;

	    IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains => this.Owner.CurrentEquipment == null
		    ? new string[0]
		    : this.Owner.CurrentEquipment.ModificationDomains;

	    public RemoveEquipmentViewModel(CommandBindingCollection commandBindings, TankViewModelBase owner)
            : base(commandBindings, owner)
        {
            this.EquipCommand = new RelayCommand(this.Equip);
        }

        private void Equip()
        {
            this.Owner.RemoveEquipment();
        }


    }
}