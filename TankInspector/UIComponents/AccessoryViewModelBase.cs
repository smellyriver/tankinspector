using System.Collections;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class AccessoryViewModelBase : NotificationObject
    {
        public abstract string Icon { get; }
        public abstract int Tier { get; }
        public abstract bool IsLoadCapable { get; }
        public abstract bool IsElite { get; }
        public abstract bool IsEquipped { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ICommand EquipCommand { get; }
        public abstract IEnumerable Replacements { get; }
        public abstract DetailedDataRelatedComponentType ComponentType { get; }

        private bool _isPreviewing;
        public bool IsPreviewing
        {
            get => _isPreviewing;
	        set
            {
                _isPreviewing = value;
                this.RaisePropertyChanged(() => this.IsPreviewing);
            }
        }

        public TankViewModelBase Owner { get; }

        public CommandBindingCollection CommandBindings { get; }

        public AccessoryViewModelBase(CommandBindingCollection commandBindings, TankViewModelBase owner)
        {
            this.CommandBindings = commandBindings;
            this.Owner = owner;
        }

        public abstract double GetWeight(double tankWeight);

    }
}
