using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class BenchmarkShellViewModel : ShellViewModel
    {

        public new BenchmarkShell Shell => (BenchmarkShell)base.Shell;

	    public override double DamagePerMinute => this.Shell.DamagePerMinute;

	    public BenchmarkShellViewModel(CommandBindingCollection commandBindings, BenchmarkShell shell, GunViewModel gun)
            : base(commandBindings, shell, gun)
        {
        }
    }
}
