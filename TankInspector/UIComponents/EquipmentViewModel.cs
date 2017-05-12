using Smellyriver.TankInspector.Modeling;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections;
using Smellyriver.Wpf.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class EquipmentViewModel : AccessoryViewModel, IEquipmentViewModel
    {

        public Equipment Equipment => (Equipment)this.Accessory;

	    public override ICommand EquipCommand { get; }

	    public override bool IsEquipped => this.Owner.LoadedEquipments.Contains(this);


	    public override IEnumerable Replacements => Owner.EquipmentReplacements;

	    protected override IEnumerable<string> ModificationDomains
        {
            get
            {
	            if (this == this.Owner.CurrentEquipment || this.Owner.CurrentEquipment== null)
                    return this.Equipment.Script.EffectiveDomains;
	            return this.Equipment.Script.EffectiveDomains.Union(this.Owner.CurrentEquipment.ModificationDomains);
            }
        }

        public override DetailedDataRelatedComponentType ComponentType => DetailedDataRelatedComponentType.Equipment;

	    public EquipmentViewModel(CommandBindingCollection commandBindings, Equipment equipment, TankViewModelBase owner)
            : base(commandBindings, equipment, owner)
        {
            this.EquipCommand = new RelayCommand(this.Equip);
        }

        

        private void Equip()
        {
            if (this.IsEquipped)
                this.Owner.RemoveEquipment(this);

            this.Owner.EquipEquipment(this);
        }

    }
}
