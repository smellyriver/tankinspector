using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class GunViewModel : DamagableModuleViewModel, ISlotViewModel, IChangableComponent, IDetailedDataRelatedComponent, ICloneable
    {

        public static readonly DataItemDescriptor<GunViewModel> ReloadTimeDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> AccuracyDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> ArmorDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> AimingTimeDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> RateOfFireDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> ElevationDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> DepressionDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> DispersionAfterShotDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> DispersionOnTurretRotationDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> DispersionOnTurretFullSpeedRotatingDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> DispersionWhileGunDamagedDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> HorizontalTraverseDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> RotationSpeedDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> MaxAmmoDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> ClipCountDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> ClipReloadTimeDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> BurstCountDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> BurstRateOfFireDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> CaliberDescriptor;

        public static readonly DataItemDescriptor<GunViewModel> HealthDescriptor;
        public static readonly DataItemDescriptor<GunViewModel> WeightDescriptor;


        static GunViewModel()
        {
            ReloadTimeDescriptor = new DataItemDescriptor<GunViewModel>("GunReloadTimeFullName", tank => tank.LoadedModules.Gun, gun => gun.ReloadTime, "UnitSeconds", "GunReloadTimeDescription", "{0:#,0.0##}", ComparisonMode.LowerBetter);

            AccuracyDescriptor = new DataItemDescriptor<GunViewModel>("GunAccuracyFullName", tank => tank.LoadedModules.Gun, gun => gun.Accuracy, "UnitMetersPer100Meters", "GunAccuracyDescription", "{0:#,0.0##}", ComparisonMode.LowerBetter);

            ArmorDescriptor = new DataItemDescriptor<GunViewModel>("GunArmorFullName", tank => tank.LoadedModules.Gun, gun => gun.Gun.GunArmor, "UnitMillimeters", "GunArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            AimingTimeDescriptor = new DataItemDescriptor<GunViewModel>("GunAimingTimeFullName", tank => tank.LoadedModules.Gun, gun => gun.AimingTime, "UnitSeconds", "GunAimingTimeDescription", "{0:#,0.0#}", ComparisonMode.LowerBetter);

            RateOfFireDescriptor = new DataItemDescriptor<GunViewModel>("GunRateOfFireFullName", tank => tank.LoadedModules.Gun, gun => gun.RateOfFire, "UnitShotsPerMinute", "GunRateOfFireDescription", "{0:#,0.00#}", ComparisonMode.HigherBetter);

            ElevationDescriptor = new DataItemDescriptor<GunViewModel>("GunElevationFullName", tank => tank.LoadedModules.Gun, gun => gun.Elevation, "UnitDegrees", "GunElevationDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            DepressionDescriptor = new DataItemDescriptor<GunViewModel>("GunDepressionFullName", tank => tank.LoadedModules.Gun, gun => gun.Depression, "UnitDegrees", "GunDepressionDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            DispersionAfterShotDescriptor = new DataItemDescriptor<GunViewModel>("GunDispersionAfterShotFullName", tank => tank.LoadedModules.Gun, gun => gun.DispersionAfterShot, "UnitFactor", "GunDispersionAfterShotDescription", "{0:#,0.00#}", ComparisonMode.LowerBetter);

            DispersionOnTurretRotationDescriptor = new DataItemDescriptor<GunViewModel>("GunDispersionOnTurretRotationFullName", tank => tank.LoadedModules.Gun, gun => gun.DispersionOnTurretRotation, "UnitFactor", "GunDispersionOnTurretRotationDescription", "{0:#,0.00#}", ComparisonMode.LowerBetter);

            DispersionOnTurretFullSpeedRotatingDescriptor = new DataItemDescriptor<GunViewModel>("GunDispersionOnTurretFullSpeedRotatingFullName", tank => tank.LoadedModules.Gun, gun => gun.DispersionOnTurretFullSpeedRotating, "UnitFactor", "GunDispersionOnTurretFullSpeedRotatingDescription", "{0:#,0.##}", ComparisonMode.LowerBetter);

            DispersionWhileGunDamagedDescriptor = new DataItemDescriptor<GunViewModel>("GunDispersionWhileGunDamagedFullName", tank => tank.LoadedModules.Gun, gun => gun.DispersionWhileGunDamaged, "UnitFactor", "GunDispersionWhileGunDamagedDescription", "{0:#,0.00#}", ComparisonMode.LowerBetter);

            HorizontalTraverseDescriptor = new DataItemDescriptor<GunViewModel>("GunHorizontalTraverseFullName", tank => tank.LoadedModules.Gun, gun => gun.HorizontalTraverse, "UnitDegrees", "GunHorizontalTraverseDescription", "{0}", ComparisonMode.HigherBetter);

            RotationSpeedDescriptor = new DataItemDescriptor<GunViewModel>("GunRotationSpeedFullName", tank => tank.LoadedModules.Gun, gun => gun.Gun.RotationSpeed, "UnitDegreesPerSecond", "GunRotationSpeedDescription", "{0:#,0.##}", ComparisonMode.HigherBetter);

            MaxAmmoDescriptor = new DataItemDescriptor<GunViewModel>("GunMaxAmmoFullName", tank => tank.LoadedModules.Gun, gun => gun.Gun.MaxAmmo, "UnitShots", "GunMaxAmmoDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            ClipCountDescriptor = new DataItemDescriptor<GunViewModel>("GunClipCountFullName", tank => tank.LoadedModules.Gun, gun => gun.ClipCount, "UnitShots", "GunClipCountDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            ClipReloadTimeDescriptor = new DataItemDescriptor<GunViewModel>("GunClipReloadTimeFullName", tank => tank.LoadedModules.Gun, gun => gun.ClipReloadTime, "UnitSeconds", "GunClipReloadTimeDescription", "{0:#,0.00#}", ComparisonMode.LowerBetter);

            BurstCountDescriptor = new DataItemDescriptor<GunViewModel>("GunBurstCountFullName", tank => tank.LoadedModules.Gun, gun => gun.BurstCount, "UnitShots", "GunBurstCountDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            BurstRateOfFireDescriptor = new DataItemDescriptor<GunViewModel>("GunBurstRateOfFireFullName", tank => tank.LoadedModules.Gun, gun => gun.BurstRateOfFire, "UnitShotsPerMinute", "GunBurstRateOfFireDescription", "{0:#,0.00#}", ComparisonMode.HigherBetter);

            CaliberDescriptor = new DataItemDescriptor<GunViewModel>("GunCaliberFullName", tank => tank.LoadedModules.Gun, gun => gun.Caliber, "UnitMillimeters", "GunCaliberDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            HealthDescriptor = new DataItemDescriptor<GunViewModel>("GunHealthFullName", tank => tank.LoadedModules.Gun, gun => gun.Health, "UnitHealthPoints", "GunHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            WeightDescriptor = new DataItemDescriptor<GunViewModel>("GunWeightFullName", tank => tank.LoadedModules.Gun, gun => gun.Gun.Weight, "UnitKilograms", "GunWeightDescription", "{0:#,0.#}", ComparisonMode.Plain);

        }

        DetailedDataRelatedComponentType IDetailedDataRelatedComponent.ComponentType => DetailedDataRelatedComponentType.Gun;

	    public IGun Gun => (IGun)this.DamagableModule;

	    private double AdditiveShotDispersionFactor => this.Owner.ModificationContext.GetValue("staticFactorDevice:miscAttrs/additiveShotDispersionFactor", "miscAttrs/additiveShotDispersionFactor", 1.0);


	    private double LoaderSkillFactor => this.Owner.ModificationContext.GetValue(LoaderSkill.SkillDomain, LoaderSkill.LoadTimeFactorSkillKey, 1.0);

	    public double Accuracy
        {
            get
            {
                var gunnerFactor = this.Owner.ModificationContext.GetValue(GunnerSkill.SkillDomain, GunnerSkill.AccuracyFactorSkillKey, 1.0);
                return this.Gun.Accuracy * gunnerFactor;
            }
        }

        private double GunnerDispersionFactor => this.Owner.ModificationContext.GetValue(GunnerSkill.SkillDomain, GunnerSkill.ShotDispersionFactorSkillKey, 1.0);

	    public double DispersionAfterShot => this.Gun.ShotDispersion.AfterShot * this.GunnerDispersionFactor * this.AdditiveShotDispersionFactor;

	    public double DispersionOnTurretRotation
        {
            get
            {
                var snapshotFactor = this.Owner.ModificationContext.GetValue(SmoothTurretSkill.SkillDomain, SmoothTurretSkill.ShotDispersionDecrementFactorSkillKey, 0.0);
                return this.Gun.ShotDispersion.TurretRotation * this.GunnerDispersionFactor * (this.AdditiveShotDispersionFactor - snapshotFactor);
            }
        }

        public double DispersionOnTurretFullSpeedRotating => this.DispersionOnTurretRotation * this.Owner.LoadedModules.Turret.Turret.RotationSpeed;

	    public double DispersionWhileGunDamaged
        {
            get
            {
                var armorerFactor = this.Owner.ModificationContext.GetValue(GunsmithSkill.SkillDomain, GunsmithSkill.ShotDispersionDecrementFactorSkillKey, 0.0);
                return this.Gun.ShotDispersion.GunDamaged * (1 - armorerFactor);
            }
        }

        public virtual double RateOfFire => 60 / (this.ReloadTime + this.ClipReloadTime * (this.ClipCount - 1)) * this.ClipCount;

	    public double ReloadTime
        {
            get
            {
                var loaderFactor = this.Owner.ModificationContext.GetValue(LoaderSkill.SkillDomain, LoaderSkill.LoadTimeFactorSkillKey, 1.0);
                var equipmentFactor = this.Owner.ModificationContext.GetValue("staticFactorDevice:miscAttrs/gunReloadTimeFactor", "miscAttrs/gunReloadTimeFactor", 1.0);
                return this.Gun.ReloadTime * loaderFactor * equipmentFactor;
            }
        }

        public double AimingTime
        {
            get
            {
                var gunnerFactor = this.Owner.ModificationContext.GetValue(GunnerSkill.SkillDomain, GunnerSkill.AimingTimeFactorSkillKey, 1.0);
                var equipmentFactor = this.Owner.ModificationContext.GetValue("staticFactorDevice:miscAttrs/gunAimingTimeFactor", "miscAttrs/gunAimingTimeFactor", 1.0);
                return this.Gun.AimingTime * gunnerFactor * equipmentFactor;
            }
        }

        public HorizontalTraverseViewModel HorizontalTraverse { get; }

        public GunElevationViewModel Elevation { get; }
        public GunDepressionViewModel Depression { get; }

        public bool IsHorizontalTraverseLimited => this.HorizontalTraverse.Range < 360;

	    public int ClipCount => this.Gun.Clip == null ? 1 : this.Gun.Clip.Count;

	    public double ClipReloadTime => this.Gun.Clip == null ? 0.0 : 60.0 / this.Gun.Clip.Rate;

	    public int BurstCount => this.Gun.Burst == null ? 1 : this.Gun.Burst.Count;

	    public double BurstRateOfFire => this.Gun.Burst == null ? 0.0 : this.Gun.Burst.Rate;

	    IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }

        private readonly ShellViewModel[] _shots;
        public IEnumerable<ShellViewModel> Shots => _shots;

	    private ShellViewModel _selectedShell;
        public ShellViewModel SelectedShell
        {
            get => _selectedShell;
	        set
            {
                _selectedShell = value;
                this.RaisePropertyChanged(() => this.SelectedShell);
            }
        }

        public bool IsMagazineGun => this.Gun.Clip != null && this.Gun.Clip.Count > 1;

	    public bool HasBurst => this.Gun.Burst != null && this.Gun.Burst.Count > 1;

	    public bool IsElite => this.Owner.IsEliteModule(this.Gun);

	    public bool IsEquipped => Owner.LoadedModules.Gun == this;

	    public IEnumerable<GunViewModel> Replacements { get; }

        System.Collections.IEnumerable IChangableComponent.Replacements => this.Replacements;

	    public string Description
        {
            get
            {
                var builder = new StringBuilder();

                var penetrationBuilder = new StringBuilder();

                for (int i = 0; i < _shots.Length; ++i)
                {
                    if (i > 0)
                        penetrationBuilder.Append("/");

                    penetrationBuilder.Append(_shots[i].Penetration.P100);
                }

                builder.Append(string.Format(App.GetLocalizedString("ColonSyntax"),
                                             App.GetLocalizedString("Penetration"),
                                             penetrationBuilder.ToString()));

                builder.AppendLine();

                var damageBuilder = new StringBuilder();

                for (int i = 0; i < _shots.Length; ++i)
                {
                    if (i > 0)
                        damageBuilder.Append("/");

                    damageBuilder.Append(_shots[i].ArmorDamage.ArmorDamage);
                }

                builder.Append(string.Format(App.GetLocalizedString("ColonSyntax"),
                                             App.GetLocalizedString("Damage"),
                                             damageBuilder.ToString()));

                return builder.ToString();
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

        public DataViewModel DataViewModel { get; private set; }

        public GunViewModel(CommandBindingCollection commandBindings, IGun gun, TankViewModelBase owner)
            : base(commandBindings, gun, owner)
        {
            this.HorizontalTraverse = new HorizontalTraverseViewModel(gun.HorizontalTraverse);
            this.Elevation = new GunElevationViewModel(gun.VerticalTraverse, gun.HorizontalTraverse);
            this.Depression = new GunDepressionViewModel(gun.VerticalTraverse, gun.HorizontalTraverse);

            this.CreateDataGroup(owner);


            this.EquipCommand = new RelayCommand(() => this.Owner.LoadGun(this.Gun.Key));
            _shots = this.Gun.Shots.Select(shell => TankObjectViewModelFactory.CreateShell(commandBindings, shell, this)).ToArray();
            this.SelectedShell = this.Shots.First();
            this.Replacements = owner.AvailableGuns.Values;
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {

            var groupViewDescriptor = new DataGroupViewDescriptor("Gun");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("RateOfFire", RateOfFireDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("ReloadTime", ReloadTimeDescriptor.SpecifySource(tank, this)));

            if (this.IsMagazineGun)
            {
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("ClipCount", ClipCountDescriptor.SpecifySource(tank, this)));
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("ClipReloadTime", ClipReloadTimeDescriptor.SpecifySource(tank, this)));
            }

            if (this.HasBurst)
            {
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("BurstCount", BurstCountDescriptor.SpecifySource(tank, this)));
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("BurstRateOfFire", BurstRateOfFireDescriptor.SpecifySource(tank, this)));
            }

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("MaxAmmo", MaxAmmoDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Accuracy", AccuracyDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("AimingTime", AimingTimeDescriptor.SpecifySource(tank, this)));

            var dispersionGroup = new ComplexDataItemViewDescriptor("GunShotDispersion");
            dispersionGroup.Items.Add(new DataItemViewDescriptor("DispersionAfterShot", DispersionAfterShotDescriptor.SpecifySource(tank, this)));
            dispersionGroup.Items.Add(new DataItemViewDescriptor("DispersionOnTurretRotation", DispersionOnTurretRotationDescriptor.SpecifySource(tank, this)));
            dispersionGroup.Items.Add(new DataItemViewDescriptor("DispersionWhileGunDamaged", DispersionWhileGunDamagedDescriptor.SpecifySource(tank, this)));
            dispersionGroup.Items.Add(new DataItemViewDescriptor("DispersionOnTurretFullSpeedRotating", DispersionOnTurretFullSpeedRotatingDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(dispersionGroup);

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Elevation", ElevationDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Depression", DepressionDescriptor.SpecifySource(tank, this)));

            if (this.IsHorizontalTraverseLimited)
                groupViewDescriptor.Items.Add(new DataItemViewDescriptor("HorizontalTraverse", HorizontalTraverseDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("RotationSpeed", RotationSpeedDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Armor", ArmorDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Weight", WeightDescriptor.SpecifySource(tank, this)));

            this.DataViewModel = new DataViewModel(groupViewDescriptor, this.Gun.Name, Owner.Owner);
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }

        public double Caliber => this.Shots.First().Shell.Caliber;

	    public GunViewModel Clone()
        {
            var clone = (GunViewModel)this.MemberwiseClone();
            clone.ClearEventHandlers();
            return clone;
        }


        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
