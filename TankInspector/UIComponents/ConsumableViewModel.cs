using Smellyriver.TankInspector.Modeling;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections;
using Smellyriver.Wpf.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class ConsumableViewModel : AccessoryViewModel, IConsumableViewModel
    {
        public Consumable Consumable => (Consumable)this.Accessory;

	    public override ICommand EquipCommand { get; }

	    public override bool IsEquipped => this.Owner.LoadedConsumables.Contains(this);

	    public override string Icon { get; }

	    public IEnumerable<string> Tags => this.Consumable.Tags;

	    public IEnumerable<string> IncompatibleTags => this.Consumable.IncompatibleTags;

	    public override IEnumerable Replacements => Owner.ConsumableReplacements;

	    protected override IEnumerable<string> ModificationDomains
        {
            get
            {
	            if (this.Owner.CurrentConsumable == null || this.Owner.CurrentConsumable == this)
                    return this.Consumable.Script.EffectiveDomains;
	            return this.Consumable.Script.EffectiveDomains.Union(this.Owner.CurrentConsumable.ModificationDomains);
            }
        }

        public override DetailedDataRelatedComponentType ComponentType => DetailedDataRelatedComponentType.Consumable;

	    public ConsumableViewModel(CommandBindingCollection commandBindings, Consumable consumable, TankViewModelBase owner)
            : base(commandBindings, consumable, owner)
        {
            this.EquipCommand = Command.FromAction(this.CommandBindings, this.Equip);
            this.Icon = $"gui/maps/icons/artefact/{consumable.Icon}.png";
        }


        private void Equip()
        {
            if (this.IsEquipped)
                this.Owner.RemoveConsumable(this);

            this.Owner.EquipConsumable(this);
        }
    }
}