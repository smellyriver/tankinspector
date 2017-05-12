using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class TurretRotatorViewModel: DamagableComponentViewModel
    {
        public static readonly DataItemDescriptor<TurretRotatorViewModel> HealthDescriptor;
        static TurretRotatorViewModel()
        {
            HealthDescriptor = new DataItemDescriptor<TurretRotatorViewModel>("TurretRotatorHealthFullName", tank => tank.LoadedModules.Turret.TurretRotator, rotator => rotator.Health, "UnitHealthPoints", "TurretRotatorHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

        }


        public DataViewModel DataViewModel { get; private set; }

        public ITurretRotator TurretRotator => (ITurretRotator)this.Component;

	    public TurretRotatorViewModel(CommandBindingCollection commandBindings, IDamageableComponent turretRotator, TurretViewModel owner)
            : base(commandBindings, turretRotator)
        {
            this.CreateDataGroup(owner.Owner);
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {
            var groupViewDescriptor = new DataGroupViewDescriptor("TurretRotator");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));
            this.DataViewModel = new DataViewModel(groupViewDescriptor, tank.Owner) { ShowHeader = false };
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }
    }
}
