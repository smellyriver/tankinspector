using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class HullViewModel : ModuleViewModel
    {
        public static readonly DataItemDescriptor<HullViewModel> FrontalArmorDescriptor;
        public static readonly DataItemDescriptor<HullViewModel> SideArmorDescriptor;
        public static readonly DataItemDescriptor<HullViewModel> RearArmorDescriptor;
        public static readonly DataItemDescriptor<HullViewModel> HealthDescriptor;
        public static readonly DataItemDescriptor<HullViewModel> WeightDescriptor;

        static HullViewModel()
        {
            FrontalArmorDescriptor = new DataItemDescriptor<HullViewModel>("HullFrontalArmorFullName", tank => tank.LoadedModules.Hull, hull => hull.Hull.FrontalArmor, "UnitMillimeters", "HullFrontalArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            SideArmorDescriptor = new DataItemDescriptor<HullViewModel>("HullSideArmorFullName", tank => tank.LoadedModules.Hull, hull => hull.Hull.SideArmor, "UnitMillimeters", "HullSideArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            RearArmorDescriptor = new DataItemDescriptor<HullViewModel>("HullRearArmorFullName", tank => tank.LoadedModules.Hull, hull => hull.Hull.RearArmor, "UnitMillimeters", "HullRearArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);


            HealthDescriptor = new DataItemDescriptor<HullViewModel>("HullHealthFullName", tank => tank.LoadedModules.Hull, hull => hull.Hull.MaxHealth, "UnitHealthPoints", "HullHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            WeightDescriptor = new DataItemDescriptor<HullViewModel>("HullWeightFullName", tank => tank.LoadedModules.Hull, hull => hull.Hull.Weight, "UnitKilograms", "HullWeightDescription", "{0:#,0}", ComparisonMode.Plain);

        }

        public IHull Hull => (IHull)this.Module;

	    public HullViewModel(CommandBindingCollection commandBindings, IHull hull, TankViewModelBase owner)
            : base(commandBindings, hull, owner)
        {

        }
    }
}
