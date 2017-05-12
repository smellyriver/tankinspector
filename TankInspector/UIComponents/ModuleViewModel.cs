using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class ModuleViewModel : TankObjectViewModel
    {
        public IModule Module => (IModule)this.TankObject;

	    public double Weight => this.Module.Weight;

	    public virtual bool IsLoadCapable => this.Owner.IsLoadCapableIfReplacedWith(this);

	    public TankViewModelBase Owner { get; }

        public ModuleViewModel(CommandBindingCollection commandBindings, IModule module, TankViewModelBase owner)
            : base(commandBindings, module)
        {
            this.Owner = owner;
        }

        internal void NotifyIsLoadCapableChanged()
        {
            this.RaisePropertyChanged(() => this.IsLoadCapable);
        }
    }
}
