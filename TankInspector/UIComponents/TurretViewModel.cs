using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Smellyriver.Wpf.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class TurretViewModel : DamagableModuleViewModel, ISlotViewModel, IChangableComponent, IDetailedDataRelatedComponent, ICloneable
    {
        public static readonly DataItemDescriptor<TurretViewModel> RotationSpeedDescriptor;
        public static readonly DataItemDescriptor<TurretViewModel> CircularVisionRadiusDescriptor;
        public static readonly DataItemDescriptor<TurretViewModel> FrontalArmorDescriptor;
        public static readonly DataItemDescriptor<TurretViewModel> SideArmorDescriptor;
        public static readonly DataItemDescriptor<TurretViewModel> RearArmorDescriptor;
        public static readonly DataItemDescriptor<TurretViewModel> HorizontalTraverseDescriptor;
        public static readonly DataItemDescriptor<TurretViewModel> HealthDescriptor;
        public static readonly DataItemDescriptor<TurretViewModel> WeightDescriptor;

        static TurretViewModel()
        {
            RotationSpeedDescriptor = new DataItemDescriptor<TurretViewModel>("TurretRotationSpeedFullName", tank => tank.LoadedModules.Turret, turret => turret.RotationSpeed, "UnitDegreesPerSecond", "TurretRotationSpeedDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            CircularVisionRadiusDescriptor = new DataItemDescriptor<TurretViewModel>("TurretCircularVisionRadiusFullName", tank => tank.LoadedModules.Turret, turret => turret.CircularVisionRadius, "UnitMeters", "TurretCircularVisionRadiusDescription", "{0:#,0.#}", ComparisonMode.HigherBetter, benchmarkRatio: 0.08);

            FrontalArmorDescriptor = new DataItemDescriptor<TurretViewModel>("TurretFrontalArmorFullName", tank => tank.LoadedModules.Turret, turret => turret.Turret.FrontalArmor, "UnitMillimeters", "TurretFrontalArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            SideArmorDescriptor = new DataItemDescriptor<TurretViewModel>("TurretSideArmorFullName", tank => tank.LoadedModules.Turret, turret => turret.Turret.SideArmor, "UnitMillimeters", "TurretSideArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            RearArmorDescriptor = new DataItemDescriptor<TurretViewModel>("TurretRearArmorFullName", tank => tank.LoadedModules.Turret, turret => turret.Turret.RearArmor, "UnitMillimeters", "TurretRearArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            HorizontalTraverseDescriptor = new DataItemDescriptor<TurretViewModel>("TurretHorizontalTraverseFullName", tank => tank.LoadedModules.Turret, turret => turret.HorizontalTraverse, "UnitDegrees", "TurretHorizontalTraverseDescription", "{0}", ComparisonMode.HigherBetter);

            HealthDescriptor = new DataItemDescriptor<TurretViewModel>("TurretHealthFullName", tank => tank.LoadedModules.Turret, turret => turret.Turret.MaxHealth, "UnitHealthPoints", "TurretHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            WeightDescriptor = new DataItemDescriptor<TurretViewModel>("TurretWeightFullName", tank => tank.LoadedModules.Turret, turret => turret.Turret.Weight, "UnitKilograms", "TurretWeightDescription", "{0:#,0}", ComparisonMode.Plain);

        }

        DetailedDataRelatedComponentType IDetailedDataRelatedComponent.ComponentType => DetailedDataRelatedComponentType.Turret;

	    public double RotationSpeed
        {
            get
            {
                var context = this.Owner.ModificationContext;
                var fuelFactor = context.GetValue("fuel", "turretRotationSpeedFactor", 1.0);
                var commandFactor = context.GetValue(GunnerSkill.SkillDomain, GunnerSkill.TurretRotationSpeedSkillKey, 1.0);
                return this.Turret.RotationSpeed * commandFactor * fuelFactor;
            }
        }

        public ITurret Turret => (ITurret)this.Module;
	    public HorizontalTraverseViewModel HorizontalTraverse { get; }

        public CircularVisionRadiusViewModel CircularVisionRadius
        {
            get
            {
                var context = this.Owner.ModificationContext;
                var equipmentFactor = context.GetValue("staticFactorDevice:miscAttrs/circularVisionRadiusFactor", "miscAttrs/circularVisionRadiusFactor", 1.0);
                var stereoscopeFactor = context.GetValue("stereoscope", "circularVisionRadiusFactor", 1.0);
                var commandFactor = context.GetValue(CommanderSkill.SkillDomain, CommanderSkill.ViewRangeFactorSkillKey, 1.0);
                var reconSkillAdditiveFactor = context.GetValue(EagleEyeSkill.SkillDomain, EagleEyeSkill.DistanceFactorPerLevelWhenDeviceWorkingSkillKey, 0.0);
                var situationalAwarenessSkillAdditiveFactor = context.GetValue(FinderSkill.SkillDomain, FinderSkill.VisionRadiusFactorSkillKey, 0.0);
                var value = this.Turret.CircularVisionRadius;
                var baseValue = value * commandFactor;
                var normalValue = Math.Min(500.0, baseValue * (equipmentFactor + reconSkillAdditiveFactor + situationalAwarenessSkillAdditiveFactor));
                if (stereoscopeFactor > 1.0)
                {
                    var secondsBeforeActivation = context.GetValue("stereoscope", "activateWhenStillSec", 3.0);
                    var stereoscopeValue = Math.Min(500.0, baseValue * (stereoscopeFactor + reconSkillAdditiveFactor + situationalAwarenessSkillAdditiveFactor));
                    return new CircularVisionRadiusViewModel(stereoscopeValue, normalValue, secondsBeforeActivation);
                }
	            return new CircularVisionRadiusViewModel(normalValue);
            }
        }

        public bool HasHorizontalTraverse => this.HorizontalTraverse.Range > 0;

	    public bool IsHorizontalTraverseLimited => this.HorizontalTraverse.Range < 360;

	    public bool IsElite => this.Owner.IsEliteModule(this.Turret);

	    public bool IsEquipped => Owner.LoadedModules.Turret == this;

	    IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }

        public string Description
        {
            get
            {
	            if (this.Turret.IsArmorDefined)
                    return string.Format(App.GetLocalizedString("ColonSyntax"),
                        App.GetLocalizedString("Armor"),
		                    $"{this.Turret.FrontalArmor.ToString("F0")}/{this.Turret.SideArmor.ToString("F0")}/{this.Turret.RearArmor.ToString("F0")}");
	            return App.GetLocalizedString("DefaultSuperstructure");
            }
        }

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

        public TurretRotatorViewModel TurretRotator { get; private set; }
        public SurveyingDeviceViewModel SurveyingDevice { get; private set; }
        public AmmoBayViewModel AmmoBay { get; private set; }

        public IEnumerable<TurretViewModel> Replacements { get; }

        System.Collections.IEnumerable IChangableComponent.Replacements => this.Replacements;

	    public DataViewModel DataViewModel { get; private set; }

        public TurretViewModel(CommandBindingCollection commandBindings, ITurret turret, TankViewModelBase owner)
            : base(commandBindings, turret, owner)
        {
            this.HorizontalTraverse = new HorizontalTraverseViewModel(turret.HorizontalTraverse);

            this.EquipCommand = new RelayCommand(() => this.Owner.LoadTurret(this.Turret));
            this.TurretRotator = new TurretRotatorViewModel(commandBindings, turret.TurretRotator, this);
            this.SurveyingDevice = new SurveyingDeviceViewModel(commandBindings, turret.SurveyingDevice, this);
            this.AmmoBay = new AmmoBayViewModel(commandBindings, this.Owner.Tank.Hull.AmmoBay, this);
            this.Replacements = this.Owner.AvailableTurrets.Values;

            this.CreateDataGroup(owner);
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {

            var groupViewDescriptor = new DataGroupViewDescriptor("Turret");

            if (this.Turret.IsArmorDefined)
            {
                var armor = new ComplexDataItemViewDescriptor("Armor");
                armor.Items.Add(new DataItemViewDescriptor("Frontal", FrontalArmorDescriptor.SpecifySource(tank, this)));
                armor.Items.Add(new DataItemViewDescriptor("Side", SideArmorDescriptor.SpecifySource(tank, this)));
                armor.Items.Add(new DataItemViewDescriptor("Rear", RearArmorDescriptor.SpecifySource(tank, this)));
                groupViewDescriptor.Items.Add(armor);
            }

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("CircularVisionRadius", CircularVisionRadiusDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            if (this.IsHorizontalTraverseLimited && this.HasHorizontalTraverse)
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("HorizontalTraverse", HorizontalTraverseDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("RotationSpeed", RotationSpeedDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Weight", WeightDescriptor.SpecifySource(tank, this)));

            this.DataViewModel = new DataViewModel(groupViewDescriptor, this.Turret.Name, this.Owner.Owner);
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }

        public bool IsCompatibleWith(IGun gun)
        {
            return this.Turret.AvailableGuns.Values.Contains(gun);
        }

        public TurretViewModel Clone()
        {
            var clone = (TurretViewModel)this.Clone();
            clone.AmmoBay = new AmmoBayViewModel(this.CommandBindings, this.AmmoBay.AmmoBay, clone);
            clone.TurretRotator = new TurretRotatorViewModel(this.CommandBindings, this.TurretRotator.TurretRotator, clone);
            clone.SurveyingDevice = new SurveyingDeviceViewModel(this.CommandBindings, this.SurveyingDevice.SurveyingDevice, clone);
            return clone;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }


    }
}
