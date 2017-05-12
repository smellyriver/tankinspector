using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal static class TankObjectViewModelFactory
    {
        public static ShellViewModel CreateShell(CommandBindingCollection commandBindings, IShell shell, GunViewModel gun)
        {
	        if (shell is BenchmarkShell)
                return new BenchmarkShellViewModel(commandBindings, (BenchmarkShell)shell, gun);
	        return new ShellViewModel(commandBindings, shell, gun);
        }

        public static TankViewModelBase CreateTank(CommandBindingCollection commandBindings, ITank tank, HangarViewModel owner)
        {
	        if (tank is Tank)
                return new TankViewModel(commandBindings, (Tank)tank, owner);
	        if (tank is BenchmarkTank)
		        return new BenchmarkTankViewModel(commandBindings, tank, owner);
	        return null;
        }
    }
}
