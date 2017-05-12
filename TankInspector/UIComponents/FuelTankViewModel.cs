using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class FuelTankViewModel : DamagableModuleViewModel
    {

        public static readonly DataItemDescriptor<FuelTankViewModel> HealthDescriptor;
        public static readonly DataItemDescriptor<FuelTankViewModel> FuelTypeDescriptor;
        public static readonly DataGroupViewDescriptor GroupViewDescriptor;
        static FuelTankViewModel()
        {
            HealthDescriptor = new DataItemDescriptor<FuelTankViewModel>("FuelTankHealthFullName", tank => tank.LoadedModules.FuelTank, fueltank => fueltank.Health, "UnitHealthPoints", "FuelTankHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            FuelTypeDescriptor = new DataItemDescriptor<FuelTankViewModel>("FuelTankFuelTypeFullName", tank => tank.LoadedModules.FuelTank, fueltank => fueltank.FuelType, null, "FuelTankFuelTypeDescription", "{0}", ComparisonMode.NotComparable);


            GroupViewDescriptor = new DataGroupViewDescriptor("FuelTank");
            GroupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor));
            GroupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            GroupViewDescriptor.Items.Add(new DataItemViewDescriptor("FuelType", FuelTypeDescriptor));
        }

        public override HealthViewModel Health
        {
            get
            {
                var factor = this.Owner.ModificationContext.GetValue("staticFactorDevice:miscAttrs/fuelTankHealthFactor", "miscAttrs/fuelTankHealthFactor", 1.0);
                var health = this.FuelTank.MaxHealth * factor;

                return new HealthViewModel(health, this.FuelTank.MaxRegenHealth);
            }
        }

        public IFuelTank FuelTank => (IFuelTank)this.Module;

	    public override string Name => App.GetLocalizedString("FuelTank");

	    public FuelTypeViewModel FuelType { get; internal set; }

        public DataViewModel DataViewModel { get; private set; }


        public FuelTankViewModel(CommandBindingCollection commandBindings, IFuelTank fuelTank, TankViewModelBase owner)
            : base(commandBindings, fuelTank, owner)
        {
            this.CreateDataGroup(owner);
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {
            var groupViewDescriptor = new DataGroupViewDescriptor("FuelTank");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("FuelType", FuelTypeDescriptor.SpecifySource(tank, this)));
            this.DataViewModel = new DataViewModel(groupViewDescriptor, tank.Owner) { ShowHeader = false };
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }
    }
}
