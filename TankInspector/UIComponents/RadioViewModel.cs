using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class RadioViewModel : DamagableModuleViewModel, ISlotViewModel, IChangableComponent, IDetailedDataRelatedComponent
    {

        public static readonly DataItemDescriptor<RadioViewModel> HealthDescriptor;
        public static readonly DataItemDescriptor<RadioViewModel> SignalDistanceDescriptor;
        public static readonly DataItemDescriptor<RadioViewModel> WeightDescriptor;

        static RadioViewModel()
        {
            HealthDescriptor = new DataItemDescriptor<RadioViewModel>("RadioHealthFullName", tank => tank.LoadedModules.Radio, radio => radio.Health, "UnitHealthPoints", "RadioHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            SignalDistanceDescriptor = new DataItemDescriptor<RadioViewModel>("RadioSignalDistanceFullName", tank => tank.LoadedModules.Radio, radio => radio.SignalDistance, "UnitMeters", "RadioSignalDistanceDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            WeightDescriptor = new DataItemDescriptor<RadioViewModel>("RadioWeightFullName", tank => tank.LoadedModules.Radio, radio => radio.Radio.Weight, "UnitKilograms", "RadioWeightDescription", "{0:#,0}", ComparisonMode.Plain);

        }

        DetailedDataRelatedComponentType IDetailedDataRelatedComponent.ComponentType => DetailedDataRelatedComponentType.Radio;

	    public IRadio Radio => (IRadio)this.Module;

	    public double SignalDistance
        {
            get
            {
                var context = this.Owner.ModificationContext;
                var radiomanFactor = context.GetValue(RadiomanSkill.SkillDomain, RadiomanSkill.SignalRangeFactorSkillKey, 1.0);
                var signalBoostingFactor = context.GetValue(InventorSkill.SkillDomain, InventorSkill.RadioDistanceFactorSkillKey, 0.0);
                return radiomanFactor * (1 + signalBoostingFactor) * this.Radio.Distance;
            }
        }

        public IEnumerable<RadioViewModel> Replacements { get; }

        System.Collections.IEnumerable IChangableComponent.Replacements => this.Replacements;

	    public bool IsElite => Owner.IsEliteModule(this.Radio);

	    public bool IsEquipped => Owner.LoadedModules.Radio == this;

	    IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }
        public string Description => string.Format(App.GetLocalizedString("ColonSyntax"), App.GetLocalizedString("RadioDistance"), this.Radio.Distance);

	    private bool _isPreviewing;
        public bool IsPreviewing
        {
            get => _isPreviewing;
	        set
            {
                _isPreviewing = value;
                this.RaisePropertyChanged(() => this.IsPreviewing);
            }
        }

        public ICommand EquipCommand { get; }

        public DataViewModel DataViewModel { get; private set; }

        public RadioViewModel(CommandBindingCollection commandBindings, IRadio radio, TankViewModelBase owner)
            : base(commandBindings, radio, owner)
        {
            this.CreateDataGroups(owner);

            this.EquipCommand = new RelayCommand(() => this.Owner.LoadRadio(this.Radio));

            this.Replacements = owner.AvailableRadios.Values;
        }

        private void CreateDataGroups(TankViewModelBase tank)
        {
            var groupViewDescriptor = new DataGroupViewDescriptor("Radio");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("SignalDistance", SignalDistanceDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Weight", WeightDescriptor.SpecifySource(tank, this)));
            this.DataViewModel = new DataViewModel(groupViewDescriptor, this.Radio.Name, this.Owner.Owner);
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }
    }
}
