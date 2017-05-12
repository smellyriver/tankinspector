using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class EngineViewModel : DamagableModuleViewModel, ISlotViewModel, IChangableComponent, IDetailedDataRelatedComponent, ICloneable
    {
        public static readonly DataItemDescriptor<EngineViewModel> EnginePowerDescriptor;
        public static readonly DataItemDescriptor<EngineViewModel> SpecificPowerDescriptor;
        public static readonly DataItemDescriptor<EngineViewModel> FireChanceDescriptor;

        public static readonly DataItemDescriptor<EngineViewModel> HealthDescriptor;
        public static readonly DataItemDescriptor<EngineViewModel> WeightDescriptor;

        static EngineViewModel()
        {
            EnginePowerDescriptor = new DataItemDescriptor<EngineViewModel>("EnginePowerFullName", tank => tank.LoadedModules.Engine, engine => engine.HorsePower, "UnitHorsePowers", "EnginePowerDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            SpecificPowerDescriptor = new DataItemDescriptor<EngineViewModel>("EngineSpecificPowerFullName", tank => tank.LoadedModules.Engine, engine => engine.SpecificPower, "UnitHorsePowersPerTon", "EngineSpecificPowerDescription", "{0:#,0.##}", ComparisonMode.HigherBetter);

            FireChanceDescriptor = new DataItemDescriptor<EngineViewModel>("EngineFireChanceFullName", tank => tank.LoadedModules.Engine, engine => engine.FireChance, null, "EngineFireChanceDescription", "{0:#,0.#%}", ComparisonMode.LowerBetter);

            HealthDescriptor = new DataItemDescriptor<EngineViewModel>("EngineHealthFullName", tank => tank.LoadedModules.Engine, engine => engine.Health, "UnitHealthPoints", "EngineHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            WeightDescriptor = new DataItemDescriptor<EngineViewModel>("EngineWeightFullName", tank => tank.LoadedModules.Engine, engine => engine.Engine.Weight, "UnitKilograms", "EngineWeightDescription", "{0:#,0.#}", ComparisonMode.Plain);

        }


        DetailedDataRelatedComponentType IDetailedDataRelatedComponent.ComponentType => DetailedDataRelatedComponentType.Engine;

	    public double HorsePower
        {
            get
            {
                var fuelFactor = this.Owner.ModificationContext.GetValue("fuel", "enginePowerFactor", 1.0);
                var removedRpmLimiterFactor = this.Owner.ModificationContext.GetValue("removedRpmLimiter", "enginePowerFactor", 1.0);
                return this.Engine.HorsePower * fuelFactor * removedRpmLimiterFactor;
            }
        }

        public double FireChance
        {
            get
            {
                var extinguisherFactor = this.Owner.ModificationContext.GetValue("extinguisher", "fireStartingChanceFactor", 1.0);
                var tidyPersonFactor = this.Owner.ModificationContext.GetValue(TidyPersonSkill.SkillDomain, TidyPersonSkill.EngineFireChanceDecrementSkillKey, 1.0);
                return this.Engine.FireChance * extinguisherFactor * tidyPersonFactor;
            }
        }

        public IEngine Engine => (IEngine)this.Module;

	    public FuelTankViewModel FuelTank { get; private set; }

        public IEnumerable<EngineViewModel> Replacements { get; }
        System.Collections.IEnumerable IChangableComponent.Replacements => this.Replacements;

	    public override HealthViewModel Health
        {
            get
            {
                var factor = this.Owner.ModificationContext.GetValue("staticFactorDevice:miscAttrs/engineHealthFactor", "miscAttrs/engineHealthFactor", 1.0);
                var health = this.Engine.MaxHealth * factor;

                return new HealthViewModel(health, this.Engine.MaxRegenHealth);
            }
        }

        public double SpecificPower => this.HorsePower / (this.Owner.Weight / 1000.0);

	    IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }

        public string Description =>
	        $"{string.Format(App.GetLocalizedString("ColonSyntax"), App.GetLocalizedString("EnginePower"), this.Engine.HorsePower)}\n{string.Format(App.GetLocalizedString("ColonSyntax"), App.GetLocalizedString("FireChance"), this.Engine.FireChance.ToString("P0"))}";

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

        public bool IsElite => this.Owner.IsEliteModule(this.Engine);

	    public bool IsEquipped => this.Owner.LoadedModules.Engine == this;

	    public ICommand EquipCommand { get; }

        public DataViewModel DataViewModel { get; private set; }

        public EngineViewModel(CommandBindingCollection commandBindings, IEngine engine, TankViewModelBase owner)
            : base(commandBindings, engine, owner)
        {
            this.CreateDataGroup(owner);

            this.EquipCommand = new RelayCommand(() => this.Owner.LoadEngine(this.Engine));
            this.FuelTank = owner.FuelTank;
            this.FuelTank.FuelType = new FuelTypeViewModel(this.Engine.FuelType);
            this.Replacements = owner.AvailableEngines.Values;
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {

            var groupViewDescriptor = new DataGroupViewDescriptor("Engine");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("EnginePower", EnginePowerDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("SpecificPower", SpecificPowerDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("FireChance", FireChanceDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Weight", WeightDescriptor.SpecifySource(tank, this)));

            this.DataViewModel = new DataViewModel(groupViewDescriptor, this.Engine.Name, this.Owner.Owner);
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }

        public EngineViewModel Clone()
        {
            var clone = (EngineViewModel)this.Clone();
            clone.FuelTank = new FuelTankViewModel(this.CommandBindings, this.FuelTank.FuelTank, clone.Owner);
            return clone;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
