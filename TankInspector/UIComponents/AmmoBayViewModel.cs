using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class AmmoBayViewModel : DamagableComponentViewModel
    {
        public static readonly DataItemDescriptor<AmmoBayViewModel> HealthDescriptor;
        static AmmoBayViewModel()
        {
            HealthDescriptor = new DataItemDescriptor<AmmoBayViewModel>("AmmoBayHealthFullName", tank => tank.LoadedModules.Turret.AmmoBay, ammoBay => ammoBay.Health, "UnitHealthPoints", "AmmoBayHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

        }

        public DataViewModel DataViewModel { get; private set; }

        public IAmmoBay AmmoBay => (IAmmoBay)this.Component;

	    public override HealthViewModel Health
        {
            get
            {
                var equipmentFactor = this.TankOwner.ModificationContext.GetValue("staticFactorDevice:miscAttrs/ammoBayHealthFactor", "miscAttrs/ammoBayHealthFactor", 1.0);
                var pedantFactor = this.TankOwner.ModificationContext.GetValue(PedantSkill.SkillDomain, PedantSkill.AmmobayHealthFactorSkillKey, 1.0);
                var health = this.AmmoBay.MaxHealth * equipmentFactor * pedantFactor;

                return new HealthViewModel(health, this.AmmoBay.MaxRegenHealth);
            }
        }

        public TurretViewModel Owner { get; }

        public TankViewModelBase TankOwner => this.Owner.Owner;

	    public AmmoBayViewModel(CommandBindingCollection commandBindings, IDamageableComponent ammoBay, TurretViewModel owner)
            : base(commandBindings, ammoBay)
        {
            this.Owner = owner;
            this.CreateDataGroup(owner.Owner);
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {
            var groupViewDescriptor = new DataGroupViewDescriptor("AmmoBay");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));
            this.DataViewModel = new DataViewModel(groupViewDescriptor, tank.Owner) { ShowHeader = false };
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }
    }
}
