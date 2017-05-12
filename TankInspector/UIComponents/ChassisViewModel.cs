using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class ChassisViewModel : DamagableModuleViewModel, ISlotViewModel, IChangableComponent, IDetailedDataRelatedComponent
    {

        public static readonly DataItemDescriptor<ChassisViewModel> RotationSpeedDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> ForwardSpeedLimitDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> BackwardSpeedLimitDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> HardTerrainResistanceDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> MediumTerrainResistanceDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> SoftTerrainResistanceDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> CanTraverseDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> BrakeForceDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> MaxClimbAngleDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> DispersionOnVehicleMoveDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> DispersionOnVehicleFullSpeedMovingDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> DispersionOnVehicleRotationDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> DispersionOnVehicleFullSpeedRotatingDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> MaxLoadDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> ArmorDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> HealthDescriptor;
        public static readonly DataItemDescriptor<ChassisViewModel> WeightDescriptor;

        static ChassisViewModel()
        {
            RotationSpeedDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisRotationSpeedFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.RotationSpeed, "UnitDegreesPerSecond", "ChassisRotationSpeedDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            ForwardSpeedLimitDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisForwardSpeedLimitFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.Chassis.SpeedLimits.Forward, "UnitKilometersPerHour", "ChassisForwardSpeedLimitDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            BackwardSpeedLimitDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisBackwardSpeedLimitFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.Chassis.SpeedLimits.Backward, "UnitKilometersPerHour", "ChassisBackwardSpeedLimitDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            HardTerrainResistanceDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisHardTerrainResistanceFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.HardTerrainResistance, "UnitMetersPerSquareSecond", "ChassisHardTerrainResistanceDescription", "{0:#,0.0##}", ComparisonMode.LowerBetter);

            MediumTerrainResistanceDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisMediumTerrainResistanceFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.MediumTerrainResistance, "UnitMetersPerSquareSecond", "ChassisMediumTerrainResistanceDescription", "{0:#,0.0##}", ComparisonMode.LowerBetter);

            SoftTerrainResistanceDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisSoftTerrainResistanceFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.SoftTerrainResistance, "UnitMetersPerSquareSecond", "ChassisSoftTerrainResistanceDescription", "{0:#,0.0##}", ComparisonMode.LowerBetter);

            CanTraverseDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisCanTraverseFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.CanTraverse, null, "ChassisCanTraverseDescription", "{0}", ComparisonMode.NotComparable, 0.0, false);

            BrakeForceDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisBrakeForceFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.Chassis.BrakeForce, "UnitNewtons", "ChassisBrakeForceDescription", "{0:#,0}", ComparisonMode.HigherBetter);

            MaxClimbAngleDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisMaxClimbAngleFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.Chassis.MaxClimbAngle, "UnitDegrees", "ChassisMaxClimbAngleDescription", "{0:#,0}", ComparisonMode.HigherBetter);

            DispersionOnVehicleMoveDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisDispersionOnVehicleMoveFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.DispersionOnMovement, "UnitFactor", "ChassisDispersionOnVehicleMoveDescription", "{0:#,0.0##}", ComparisonMode.LowerBetter);

            DispersionOnVehicleFullSpeedMovingDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisDispersionOnVehicleFullSpeedMovingFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.DispersionOnFullSpeedMoving, "UnitFactor", "ChassisDispersionOnVehicleFullSpeedMovingDescription", "{0:0.##}", ComparisonMode.LowerBetter);

            DispersionOnVehicleRotationDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisDispersionOnVehicleRotationFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.DispersionOnRotation, "UnitFactor", "ChassisDispersionOnVehicleRotationDescription", "{0:#,0.0##}", ComparisonMode.LowerBetter);

            DispersionOnVehicleFullSpeedRotatingDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisDispersionOnVehicleFullSpeedRotatingFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.DispersionOnFullSpeedRotating, "UnitFactor", "ChassisDispersionOnVehicleFullSpeedRotatingDescription", "{0:#,0.##}", ComparisonMode.LowerBetter);

            MaxLoadDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisMaxLoadFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.MaxLoad, "UnitKilograms", "ChassisMaxLoadDescription", "{0:#,0}", ComparisonMode.HigherBetter);

            HealthDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisHealthFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.Health, "UnitHealthPoints", "ChassisHealthDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

            WeightDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisWeightFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.Chassis.Weight, "UnitKilograms", "ChassisWeightDescription", "{0:#,0}", ComparisonMode.Plain);

            ArmorDescriptor = new DataItemDescriptor<ChassisViewModel>("ChassisArmorFullName", tank => tank.LoadedModules.Chassis, chassis => chassis.TrackArmor, "UnitMillimeters", "ChassisArmorDescription", "{0:#,0.#}", ComparisonMode.HigherBetter);

        }


        DetailedDataRelatedComponentType IDetailedDataRelatedComponent.ComponentType => DetailedDataRelatedComponentType.Chassis;

	    public double RotationSpeed
        {
            get
            {
                var virtuosoFactor = this.Owner.ModificationContext.GetValue(VirtuosoSkill.SkillDomain, VirtuosoSkill.ChassisRotationSpeedIncrementFactor, 0.0);
                return this.Chassis.RotationSpeed * (1 + virtuosoFactor);
            }
        }

        private double AdditiveShotDispersionFactor => this.Owner.ModificationContext.GetValue("staticFactorDevice:miscAttrs/additiveShotDispersionFactor", "miscAttrs/additiveShotDispersionFactor", 1.0);

	    public double DispersionOnMovement
        {
            get
            {
                var gunnerFactor = this.Owner.ModificationContext.GetValue(GunnerSkill.SkillDomain, GunnerSkill.ShotDispersionFactorSkillKey, 1.0);
                var smoothDrivingFactor = this.Owner.ModificationContext.GetValue(SmoothDrivingSkill.SkillDomain, SmoothDrivingSkill.ShotDispersionDecrementFactorSkillKey, 0.0);
                return this.Chassis.ShotDispersion.Movement * gunnerFactor * this.AdditiveShotDispersionFactor * (1 - smoothDrivingFactor);
            }
        }

        public double DispersionOnFullSpeedMoving
        {
            get
            {
                var speedLimits = this.Owner.LoadedModules.Chassis.Chassis.SpeedLimits;
                var fullSpeed = Math.Max(speedLimits.Forward, speedLimits.Backward);
                return this.DispersionOnMovement * fullSpeed;
            }
        }
        
        public double DispersionOnRotation
        {
            get
            {
                var gunnerFactor = this.Owner.ModificationContext.GetValue(GunnerSkill.SkillDomain, GunnerSkill.ShotDispersionFactorSkillKey, 1.0);
                return this.Chassis.ShotDispersion.Rotation * gunnerFactor * this.AdditiveShotDispersionFactor;
            }
        }

        public double DispersionOnFullSpeedRotating => this.DispersionOnRotation * this.Chassis.RotationSpeed;

	    public override bool IsLoadCapable => this.Owner.IsLoadCapableIfReplacedWith(this);

	    public IChassis Chassis => (IChassis)this.Module;
	    public double TrackArmor => this.Chassis.TrackArmor;


	    public double MaxLoad
        {
            get
            {
                var factor = this.Owner.ModificationContext.GetValue("enhancedSuspension", "chassisMaxLoadFactor", 1.0);
                return this.Chassis.MaxLoad * factor;
            }
        }

        public override HealthViewModel Health
        {
            get
            {
                var factor = this.Owner.ModificationContext.GetValue("enhancedSuspension", "chassisHealthFactor", 1.0);
                return new HealthViewModel(this.Chassis.MaxHealth * factor, this.Chassis.MaxRegenHealth);
            }
        }

        public double SoftTerrainResistance
        {
            get
            {
                var context = this.Owner.ModificationContext;
                var driverFactor = context.GetValue(DriverSkill.SkillDomain, DriverSkill.TerrainResistanceFactorSkillKey, 1.0);
                var baseValue = this.Chassis.TerrainResistance.SoftTerrain * driverFactor;
                var offRoadDrivingFactor = context.GetValue(BadRoadsKingSkill.SkillDomain, BadRoadsKingSkill.SoftGroundResistanceFactorDecrementSkillKey, 0.0);
                var grousersFactor = context.GetValue("grousers", "softGroundResistanceFactor", 1.0);

                return baseValue * (grousersFactor - offRoadDrivingFactor);
            }
        }

        public double MediumTerrainResistance
        {
            get
            {
                var context = this.Owner.ModificationContext;
                var driverFactor = context.GetValue(DriverSkill.SkillDomain, DriverSkill.TerrainResistanceFactorSkillKey, 1.0);
                var baseValue = this.Chassis.TerrainResistance.MediumTerrain * driverFactor;
                var offRoadDrivingFactor = context.GetValue(BadRoadsKingSkill.SkillDomain, BadRoadsKingSkill.MediumGroundResistanceFactorDecrementSkillKey, 0.0);
                var grousersFactor = context.GetValue("grousers", "mediumGroundResistanceFactor", 1.0);

                return baseValue * (grousersFactor - offRoadDrivingFactor);
            }
        }

        public double HardTerrainResistance
        {
            get
            {
                var context = this.Owner.ModificationContext;
                var driverFactor = context.GetValue(DriverSkill.SkillDomain, DriverSkill.TerrainResistanceFactorSkillKey, 1.0);
                var baseValue = this.Chassis.TerrainResistance.HardTerrain * driverFactor;

                return baseValue;
            }
        }

        public CanTraverseViewModel CanTraverse { get; }

        IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains
        {
            get { yield break; }
        }

        public IEnumerable<ChassisViewModel> Replacements { get; }

        System.Collections.IEnumerable IChangableComponent.Replacements => this.Replacements;

	    public string Description =>
		    $"{string.Format(App.GetLocalizedString("ColonSyntax"), App.GetLocalizedString("RotationSpeed"), this.Chassis.RotationSpeed)}\n{string.Format(App.GetLocalizedString("ColonSyntax"), App.GetLocalizedString("MaxLoad"), this.Chassis.MaxLoad)}";

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
        public bool IsEquipped => Owner.LoadedModules.Chassis == this;

	    public bool IsElite => Owner.IsEliteModule(this.Chassis);

	    public ICommand EquipCommand { get; }

        public DataViewModel DataViewModel { get; private set; }

        public ChassisViewModel(CommandBindingCollection commandBindings, IChassis chassis, TankViewModelBase owner)
            : base(commandBindings, chassis, owner)
        {
            this.CanTraverse = new CanTraverseViewModel(chassis.CanTraverse);
            this.CreateDataGroup(owner);
            this.EquipCommand = new RelayCommand(() => this.Owner.LoadChassis(this.Chassis));
            this.Replacements = owner.AvailableChassis.Values;
        }

        private void CreateDataGroup(TankViewModelBase tank)
        {

            var groupViewDescriptor = new DataGroupViewDescriptor("Chassis");
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("RotationSpeed", RotationSpeedDescriptor.SpecifySource(tank, this)));

            var speedLimits = new ComplexDataItemViewDescriptor("SpeedLimits");
            speedLimits.Items.Add(new DataItemViewDescriptor("Forward", ForwardSpeedLimitDescriptor.SpecifySource(tank, this)));
            speedLimits.Items.Add(new DataItemViewDescriptor("Backward", BackwardSpeedLimitDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(speedLimits);

            var terrainResistances = new ComplexDataItemViewDescriptor("TerrainResistance");
            terrainResistances.Items.Add(new DataItemViewDescriptor("OnHardTerrain", HardTerrainResistanceDescriptor.SpecifySource(tank, this)));
            terrainResistances.Items.Add(new DataItemViewDescriptor("OnMediumTerrain", MediumTerrainResistanceDescriptor.SpecifySource(tank, this)));
            terrainResistances.Items.Add(new DataItemViewDescriptor("OnSoftTerrain", SoftTerrainResistanceDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(terrainResistances);

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("CanTraverse", CanTraverseDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("BrakeForce", BrakeForceDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("MaxClimbAngle", MaxClimbAngleDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            var shotDispersions = new ComplexDataItemViewDescriptor("GunShotDispersion");
            shotDispersions.Items.Add(new DataItemViewDescriptor("DispersionOnVehicleMove", DispersionOnVehicleMoveDescriptor.SpecifySource(tank, this)));
            shotDispersions.Items.Add(new DataItemViewDescriptor("DispersionOnVehicleRotation", DispersionOnVehicleRotationDescriptor.SpecifySource(tank, this)));
            shotDispersions.Items.Add(new DataItemViewDescriptor("DispersionOnVehicleFullSpeedMoving", DispersionOnVehicleFullSpeedMovingDescriptor.SpecifySource(tank, this)));
            shotDispersions.Items.Add(new DataItemViewDescriptor("DispersionOnVehicleFullSpeedRotating", DispersionOnVehicleFullSpeedRotatingDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(shotDispersions);

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("MaxLoad", MaxLoadDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());


            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Armor", ArmorDescriptor.SpecifySource(tank, this)));
            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Health", HealthDescriptor.SpecifySource(tank, this)));

            groupViewDescriptor.Items.Add(new DataSeparatorDescriptor());

            groupViewDescriptor.Items.Add(new DataItemViewDescriptor("Weight", WeightDescriptor.SpecifySource(tank, this)));

            this.DataViewModel = new DataViewModel(groupViewDescriptor, this.Chassis.Name, this.Owner.Owner);
            this.DataViewModel.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;

            this.DataViewModel.Tank = tank;
        }

    }
}
