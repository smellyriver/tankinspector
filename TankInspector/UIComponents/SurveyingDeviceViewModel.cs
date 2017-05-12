using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class SurveyingDeviceViewModel : DamagableComponentViewModel
    {
        public static readonly DataItemDescriptor<SurveyingDeviceViewModel> HealthDescriptor;
        static SurveyingDeviceViewModel()
        {
            HealthDescriptor = new DataItemDescriptor<SurveyingDeviceViewModel>("SurveyingDeviceHealthFullName", tank => tank.LoadedModules.Turret.SurveyingDevice, device => device.Health, "UnitHealthPoints", "SurveyingDeviceHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

        }

        public DataViewModel DataViewModel { get; private set; }

        public ISurveyingDevice SurveyingDevice => (ISurveyingDevice)this.Component;

	    public SurveyingDeviceViewModel(CommandBindingCollection commandBindings, IDamageableComponent turretRotator, TurretViewModel owner)
            : base(commandBindings, turretRotator)
        {
            this.CreateDataGroup(owner.Owner);
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {
            var groupViewDescriptor = new DataGroupViewDescriptor("SurveyingDevice");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));
            this.DataViewModel = new DataViewModel(groupViewDescriptor, tank.Owner) { ShowHeader = false };
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }
    }
}
