using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class DamagableComponentViewModel : NotificationObject
    {
        public IDamageableComponent Component { get; }

        public string Name => this.Component.Name;
	    public virtual HealthViewModel Health { get; }
        protected CommandBindingCollection CommandBindings { get; }

        public DamagableComponentViewModel(CommandBindingCollection commandBindings, IDamageableComponent component)
        {
            this.Health = new HealthViewModel(component.MaxHealth, component.MaxRegenHealth);
            this.CommandBindings = commandBindings;
            this.Component = component;
        }
    }
}
