using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class DamagableModuleViewModel : ModuleViewModel
    {

        public IDamageableModule DamagableModule => (IDamageableModule)this.Module;

	    public virtual HealthViewModel Health { get; }

        public DamagableModuleViewModel(CommandBindingCollection commandBindings, IDamageableModule module, TankViewModelBase owner)
            : base(commandBindings, module, owner)
        {
            this.Health = new HealthViewModel(module.MaxHealth, module.MaxRegenHealth);
        }
    }
}
