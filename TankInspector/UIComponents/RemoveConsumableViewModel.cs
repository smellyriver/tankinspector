using Smellyriver.Wpf.Input;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class RemoveConsumableViewModel : EmptyAccessoryViewModel, IConsumableViewModel
    {

        public override string Name => App.GetLocalizedString("RemoveConsumable");

	    public override string Description => App.GetLocalizedString("RemoveConsumableDescription");

	    public override ICommand EquipCommand { get; }

	    public override IEnumerable Replacements => Owner.ConsumableReplacements;

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
            get
            {
	            if (this.Owner.CurrentConsumable == null)
                    return new string[0];
	            return this.Owner.CurrentConsumable.ModificationDomains;
            }
        }

        public RemoveConsumableViewModel(CommandBindingCollection commandBindings, TankViewModelBase owner)
            : base(commandBindings, owner)
        {
            this.EquipCommand = new RelayCommand(this.Equip);
        }

        private void Equip()
        {
            this.Owner.RemoveConsumable();
        }

    }
}
