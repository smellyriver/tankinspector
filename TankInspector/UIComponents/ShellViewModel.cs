using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class ShellViewModel : CommodityViewModel, ICloneable, ISlotViewModel, IChangableComponent, IDetailedDataRelatedComponent
    {
        public static readonly DataItemDescriptor<ShellViewModel> CaliberDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> ExplosionRadiusDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> SpeedDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> GravityDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> MaxDistanceDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> PenetrationDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> ArmorDamageDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> DamagePerMinuteDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> ModuleDamageDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> PriceDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> PricePerDamageDescriptor;
        public static readonly DataItemDescriptor<ShellViewModel> PiercingPowerLossFactorByDistanceDescriptor;

        static ShellViewModel()
        {
            CaliberDescriptor = new DataItemDescriptor<ShellViewModel>("ShellCaliberFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Shell.Caliber, "UnitMillimeters", "ShellCaliberDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            ExplosionRadiusDescriptor = new DataItemDescriptor<ShellViewModel>("ShellExplosionRadiusFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Shell.ExplosionRadius, "UnitMeters", "ShellExplosionRadiusDescription", "{0:#,0.###}", ComparisonMode.HigherBetter);

            SpeedDescriptor = new DataItemDescriptor<ShellViewModel>("ShellSpeedFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Shell.Speed, "UnitMetersPerSecond", "ShellSpeedDescription", "{0:#,0}", ComparisonMode.HigherBetter);

            GravityDescriptor = new DataItemDescriptor<ShellViewModel>("ShellGravityFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Shell.Gravity, "UnitMetersPerSquareSecond", "ShellGravityDescription", "{0:#,0.##}", ComparisonMode.Plain);

            MaxDistanceDescriptor = new DataItemDescriptor<ShellViewModel>("ShellMaxDistanceFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Shell.MaxDistance, "UnitMeters", "ShellMaxDistanceDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            PenetrationDescriptor = new DataItemDescriptor<ShellViewModel>("ShellPenetrationFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Penetration, "UnitMillimeters", "ShellPenetrationDescription", "{0}", ComparisonMode.HigherBetter);

            ArmorDamageDescriptor = new DataItemDescriptor<ShellViewModel>("ShellArmorDamageFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.ArmorDamage, "UnitHealthPoints", "ShellArmorDamageDescription", "{0}", ComparisonMode.HigherBetter);

            DamagePerMinuteDescriptor = new DataItemDescriptor<ShellViewModel>("ShellDamagePerMinuteFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.DamagePerMinute, "UnitHealthPointsPerMinute", "ShellDamagePerMinuteDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            ModuleDamageDescriptor = new DataItemDescriptor<ShellViewModel>("ShellModuleDamageFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Shell.Damage.Devices, "UnitHealthPoints", "ShellModuleDamageDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            PriceDescriptor = new DataItemDescriptor<ShellViewModel>("ShellPriceFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Price, null, "ShellPriceDescription", "{0:#,0.#}", ComparisonMode.LowerBetter);

            PricePerDamageDescriptor = new DataItemDescriptor<ShellViewModel>("ShellPricePerDamageFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.PricePerDamage, null, "ShellPricePerDamageDescription", "{0}", ComparisonMode.LowerBetter);

            PiercingPowerLossFactorByDistanceDescriptor = new DataItemDescriptor<ShellViewModel>("ShellPiercingPowerLossFactorByDistanceFullName", tank => tank.LoadedModules.Gun.SelectedShell, shell => shell.Shell.PiercingPowerLossFactorByDistance, "UnitPerMeter", "ShellPiercingPowerLossFactorByDistanceDescription", "{0:#,0.#%}", ComparisonMode.LowerBetter);

        }

        DetailedDataRelatedComponentType IDetailedDataRelatedComponent.ComponentType => DetailedDataRelatedComponentType.Shell;

	    public IShell Shell => (IShell)this.Commodity;

	    public string Description =>
		    $"{string.Format(App.GetLocalizedString("ColonSyntax"), App.GetLocalizedString("Penetration"), this.Penetration.P100)}\n{string.Format(App.GetLocalizedString("ColonSyntax"), App.GetLocalizedString("Damage"), this.Shell.Damage.Armor)}";

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

        IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }

        public PricePerDamageViewModel PricePerDamage { get; }

        public string Name => this.Shell.Name;
	    public ShellArmorDamageViewModel ArmorDamage { get; }
        public PiercingPowerViewModel Penetration { get; }
        public bool HasPiercingPowerLossFactor => this.Shell.PiercingPowerLossFactorByDistance > 0;

	    public IEnumerable<ShellViewModel> Replacements => _gun.Shots;

	    System.Collections.IEnumerable IChangableComponent.Replacements => this.Replacements;

	    public bool HasExplosionRadius => this.Shell.ExplosionRadius > double.Epsilon;

	    public bool IsElite => this.Commodity.CurrencyType == CurrencyType.Gold;

	    public bool IsEquipped => _gun.SelectedShell == this;

	    public bool IsLoadCapable => true;

	    public int Tier => -1;

	    public int MaxAmmo => _gun.Gun.MaxAmmo;

	    public virtual double DamagePerMinute => _gun.RateOfFire * this.Shell.Damage.Armor;

	    public ICommand EquipCommand { get; }

        private readonly GunViewModel _gun;

        public DataViewModel DataViewModel { get; private set; }
        public DataViewModel CompactDataViewModel { get; private set; }

        public ShellViewModel(CommandBindingCollection commandBindings, IShell shell, GunViewModel gun)
            : base(commandBindings, shell)
        {

            this.ArmorDamage = new ShellArmorDamageViewModel(shell.Damage.Armor);
            this.Penetration = new PiercingPowerViewModel(shell.PiercingPower);
            this.PricePerDamage = new PricePerDamageViewModel((double)shell.Price / shell.Damage.Armor, shell.CurrencyType);

            this.CreateDataGroups(gun.Owner);
            this.EquipCommand = new RelayCommand(() => _gun.SelectedShell = this);
            _gun = gun;
        }

        private void CreateDataGroups(TankViewModelBase tank)
        {

            var groupViewDescriptor = new DataGroupViewDescriptor("Shell");
            var compactGroupViewDescriptor = new DataGroupViewDescriptor("Shell");

            var armorDamage = new DataItemViewDescriptor("Damage", ArmorDamageDescriptor.SpecifySource(tank, this));
            groupViewDescriptor.Items.Add(armorDamage);
            compactGroupViewDescriptor.Items.Add(armorDamage);

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("DamagePerMinute", DamagePerMinuteDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("ModuleDamage", ModuleDamageDescriptor.SpecifySource(tank, this)));

            if (this.HasExplosionRadius)
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("ExplosionRadius", ExplosionRadiusDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            var penetration = new DataItemViewDescriptor("Penetration", PenetrationDescriptor.SpecifySource(tank, this));
            groupViewDescriptor.Items.Add(penetration);
            compactGroupViewDescriptor.Items.Add(penetration);

            if (this.HasPiercingPowerLossFactor)
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("PenetrationLossFactorByDistance", PiercingPowerLossFactorByDistanceDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("MaxDistance", MaxDistanceDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Speed", SpeedDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Gravity", GravityDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Price", PriceDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("PricePerDamage", PricePerDamageDescriptor.SpecifySource(tank, this)));
            

            this.DataViewModel = new DataViewModel(groupViewDescriptor, this.Name, tank.Owner) { ShowHeader = false };
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;
            this.DataViewModel.Tank = tank;

            this.CompactDataViewModel = new DataViewModel(compactGroupViewDescriptor, this.Name, tank.Owner) { ShowHeader = false };
            this.CompactDataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;
            this.CompactDataViewModel.Tank = tank;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        
    }
}