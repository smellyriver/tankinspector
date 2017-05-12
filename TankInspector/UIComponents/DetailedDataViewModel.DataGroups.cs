using Smellyriver.TankInspector.DataAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Smellyriver.Utilities;
using Smellyriver.TankInspector.Modeling;
using System.Globalization;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class DetailedDataViewModel
    {
        public enum DisplayPriority : int
        {
            All = -1000,
            DefaultHidden = -200,
            NotThatImportant = -100,
            Normal = 0,
        }


        private static Dictionary<DetailedDataRelatedComponentType, T> CreateRelationshipDictionary<T>()
            where T : new()
        {
            var dictionary = new Dictionary<DetailedDataRelatedComponentType, T>();

            dictionary.Add(DetailedDataRelatedComponentType.Chassis, new T());
            dictionary.Add(DetailedDataRelatedComponentType.Engine, new T());
            dictionary.Add(DetailedDataRelatedComponentType.Gun, new T());
            dictionary.Add(DetailedDataRelatedComponentType.Radio, new T());
            dictionary.Add(DetailedDataRelatedComponentType.Shell, new T());
            dictionary.Add(DetailedDataRelatedComponentType.Turret, new T());

            return dictionary;
        }

        private DataViewModel[] _allDataViewModels;
        private List<DataItem> _allDataItems;
        private List<ComplexDataItemViewModel> _allComplexDataItemVMs;
        private Dictionary<DetailedDataRelatedComponentType, List<DataItem>> _dataItemComponentRelationships;
        private Dictionary<string, List<DataItem>> _dataItemModificationDomainRelationships;
        private Dictionary<DataItemDescriptorBase, Collection<DetailedDataRelatedComponentType>> _dataItemDescriptorComponentTypeRelationships;
        private Dictionary<DataItemDescriptorBase, Collection<string>> _dataItemDescriptorModificationDomainRelationships;


        private DataViewModel _mobilityDataGroup;
        public DataViewModel MobilityDataGroup
        {
            get => _mobilityDataGroup;
	        private set
            {
                _mobilityDataGroup = value;
                this.RaisePropertyChanged(() => this.MobilityDataGroup);
            }
        }
        private DataViewModel _survivabilityDataGroup;
        public DataViewModel SurvivabilityDataGroup
        {
            get => _survivabilityDataGroup;
	        private set
            {
                _survivabilityDataGroup = value;
                this.RaisePropertyChanged(() => this.SurvivabilityDataGroup);
            }
        }

        private DataViewModel _scoutabilityDataGroup;
        public DataViewModel ScoutabilityDataGroup
        {
            get => _scoutabilityDataGroup;
	        private set
            {
                _scoutabilityDataGroup = value;
                this.RaisePropertyChanged(() => this.ScoutabilityDataGroup);
            }
        }

        private DataViewModel _economyDataGroup;
        public DataViewModel EconomyDataGroup
        {
            get => _economyDataGroup;
	        private set
            {
                _economyDataGroup = value;
                this.RaisePropertyChanged(() => this.EconomyDataGroup);
            }
        }

        private DataViewModel _battleDataGroup;
        public DataViewModel BattleDataGroup
        {
            get => _battleDataGroup;
	        private set
            {
                _battleDataGroup = value;
                this.RaisePropertyChanged(() => this.BattleDataGroup);
            }
        }

        private DataViewModel _miscellaneousDataGroup;
        public DataViewModel MiscellaneousDataGroup
        {
            get => _miscellaneousDataGroup;
	        private set
            {
                _miscellaneousDataGroup = value;
                this.RaisePropertyChanged(() => this.MiscellaneousDataGroup);
            }
        }

        private DataViewModel _firePowerDataGroup;
        public DataViewModel FirePowerDataGroup
        {
            get => _firePowerDataGroup;
	        private set
            {
                _firePowerDataGroup = value;
                this.RaisePropertyChanged(() => this.FirePowerDataGroup);
            }
        }

        private DataViewModel _maneuverabilityDataGroup;
        public DataViewModel ManeuverabilityDataGroup
        {
            get => _maneuverabilityDataGroup;
	        set
            {
                _maneuverabilityDataGroup = value;
                this.RaisePropertyChanged(() => this.ManeuverabilityDataGroup);
            }
        }

        private void AddRelationship(DataItemDescriptorBase descriptor, DetailedDataRelatedComponentType relatedComponentType)
        {
            var collection = _dataItemDescriptorComponentTypeRelationships.GetOrCreate(descriptor, () => new Collection<DetailedDataRelatedComponentType>());
            collection.Add(relatedComponentType);
        }

        private void AddRelationship(DataItemDescriptorBase descriptor, string modifierDomain)
        {
            var collection = _dataItemDescriptorModificationDomainRelationships.GetOrCreate(descriptor, () => new Collection<string>());
            collection.Add(modifierDomain);
        }

        private void AddCrewSkillLevelRelationship(DataItemDescriptorBase descriptor)
        {
            AddRelationship(descriptor, "staticAdditiveDevice:miscAttrs/crewLevelIncrease");
            AddRelationship(descriptor, "stimulator");
            AddRelationship(descriptor, BrotherhoodSkill.SkillDomain);
        }

        private DataViewModel CreateMiscellaneousDataGroup()
        {
            var miscellaneousViewDescriptor = new DataGroupViewDescriptor("Miscellaneous");

            var weight = new ComplexDataItemViewDescriptor("Weight");

            var vehicleWeight = new DataItemViewDescriptor("VehicleWeight", TankViewModelBase.WeightDescriptor, 0);
            AddRelationship(vehicleWeight.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(vehicleWeight.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(vehicleWeight.Descriptor, DetailedDataRelatedComponentType.Engine);
            AddRelationship(vehicleWeight.Descriptor, DetailedDataRelatedComponentType.Radio);
            AddRelationship(vehicleWeight.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(vehicleWeight.Descriptor, DetailedDataRelatedComponentType.Equipment);
            weight.Items.Add(vehicleWeight);

            var hullWeight = new DataItemViewDescriptor("HullWeight", HullViewModel.WeightDescriptor, -150);
            weight.Items.Add(hullWeight);

            var turretWeight = new DataItemViewDescriptor("TurretWeight", TurretViewModel.WeightDescriptor, -50);
            AddRelationship(turretWeight.Descriptor, DetailedDataRelatedComponentType.Turret);
            weight.Items.Add(turretWeight);

            var gunWeight = new DataItemViewDescriptor("GunWeight", GunViewModel.WeightDescriptor, -75);
            AddRelationship(gunWeight.Descriptor, DetailedDataRelatedComponentType.Gun);
            weight.Items.Add(gunWeight);

            var engineWeight = new DataItemViewDescriptor("EngineWeight", EngineViewModel.WeightDescriptor, -90);
            AddRelationship(engineWeight.Descriptor, DetailedDataRelatedComponentType.Engine);
            weight.Items.Add(engineWeight);

            var chassisWeight = new DataItemViewDescriptor("ChassisWeight", ChassisViewModel.WeightDescriptor, -125);
            AddRelationship(chassisWeight.Descriptor, DetailedDataRelatedComponentType.Chassis);
            weight.Items.Add(chassisWeight);

            var radioWeight = new DataItemViewDescriptor("RadioWeight", RadioViewModel.WeightDescriptor, -110);
            AddRelationship(radioWeight.Descriptor, DetailedDataRelatedComponentType.Radio);
            weight.Items.Add(radioWeight);

            var equipmentWeight = new DataItemViewDescriptor("EquipmentWeight", TankViewModelBase.EquipmentWeightDescriptor, -60);
            AddRelationship(equipmentWeight.Descriptor, DetailedDataRelatedComponentType.Equipment);
            weight.Items.Add(equipmentWeight);
            miscellaneousViewDescriptor.Items.Add(weight);

            var maxLoad = new DataItemViewDescriptor("MaxLoad", ChassisViewModel.MaxLoadDescriptor, 10);
            AddRelationship(maxLoad.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(maxLoad.Descriptor, "enhancedSuspension");
            miscellaneousViewDescriptor.Items.Add(maxLoad);

            return new DataViewModel(miscellaneousViewDescriptor);
        }

        private DataViewModel CreateBattleDataGroup()
        {
            var battleViewDescriptor = new DataGroupViewDescriptor("Battle");


            var matchmaking = new ComplexDataItemViewDescriptor("Matchmaking");
            var battleTiers = new DataItemViewDescriptor("TankBattleTiers", TankViewModelBase.BattleTiersDescriptor, 10);
            matchmaking.Items.Add(battleTiers);
            var mmWeight = new DataItemViewDescriptor("TankMatchmakingWeight",
                                                      TankViewModelBase.MatchmakingWeightDescriptor,
                                                      20,
                                                      v => v.Tank.MatchMakingWeight > 0);
            matchmaking.Items.Add(mmWeight);

            battleViewDescriptor.Items.Add(matchmaking);

            return new DataViewModel(battleViewDescriptor);
        }

        private DataViewModel CreateEconomyDataGroup()
        {
            var economyViewDescriptor = new DataGroupViewDescriptor("Economy");

            var tankPrice = new DataItemViewDescriptor("TankPrice", TankViewModelBase.PriceDescriptor, -50);
            economyViewDescriptor.Items.Add(tankPrice);

            var shellPrice = new DataItemViewDescriptor("ShellPrice", ShellViewModel.PriceDescriptor, -20);
            AddRelationship(shellPrice.Descriptor, DetailedDataRelatedComponentType.Shell);
            economyViewDescriptor.Items.Add(shellPrice);

            var shellPricePerDamage = new DataItemViewDescriptor("ShellPricePerDamage", ShellViewModel.PricePerDamageDescriptor, -25);
            AddRelationship(shellPricePerDamage.Descriptor, DetailedDataRelatedComponentType.Shell);
            economyViewDescriptor.Items.Add(shellPricePerDamage);

            var averageCost = new ComplexDataItemViewDescriptor("TankOperationCost");
            var repairCost = new DataItemViewDescriptor("TankMaxRepairCost", TankViewModelBase.RepairCostDescriptor, -120);
            AddRelationship(repairCost.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(repairCost.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(repairCost.Descriptor, DetailedDataRelatedComponentType.Engine);
            AddRelationship(repairCost.Descriptor, DetailedDataRelatedComponentType.Radio);
            AddRelationship(repairCost.Descriptor, DetailedDataRelatedComponentType.Chassis);
            averageCost.Items.Add(repairCost);

            return new DataViewModel(economyViewDescriptor);
        }

        private DataViewModel CreateScoutabilityDataGroup()
        {
            var scoutabilityViewDescriptor = new DataGroupViewDescriptor("Scoutability");

            var circularVisionRadius = new DataItemViewDescriptor("CircularVisionRadius", TurretViewModel.CircularVisionRadiusDescriptor, 50);
            AddRelationship(circularVisionRadius.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(circularVisionRadius.Descriptor, CommanderSkill.SkillDomain);
            AddRelationship(circularVisionRadius.Descriptor, EagleEyeSkill.SkillDomain);
            AddRelationship(circularVisionRadius.Descriptor, FinderSkill.SkillDomain);
            AddCrewSkillLevelRelationship(circularVisionRadius.Descriptor);
            AddRelationship(circularVisionRadius.Descriptor, "staticFactorDevice:miscAttrs/circularVisionRadiusFactor");
            AddRelationship(circularVisionRadius.Descriptor, "stereoscope");
            scoutabilityViewDescriptor.Items.Add(circularVisionRadius);

            var signalDistance = new DataItemViewDescriptor("SignalDistance", RadioViewModel.SignalDistanceDescriptor, -90);
            AddRelationship(signalDistance.Descriptor, DetailedDataRelatedComponentType.Radio);
            AddRelationship(signalDistance.Descriptor, RadiomanSkill.SkillDomain);
            AddRelationship(signalDistance.Descriptor, InventorSkill.SkillDomain);
            AddCrewSkillLevelRelationship(signalDistance.Descriptor);
            scoutabilityViewDescriptor.Items.Add(signalDistance);

            var camouflageValue = new ComplexDataItemViewDescriptor("CamouflageValues");

            var stationaryCamouflage = new DataItemViewDescriptor("StationaryCamouflageValue", TankViewModelBase.StationaryCamouflageDescriptor, 40);
            AddRelationship(stationaryCamouflage.Descriptor, "camouflageNet");
            AddRelationship(stationaryCamouflage.Descriptor, CamouflageSkill.SkillDomain);
            camouflageValue.Items.Add(stationaryCamouflage);
            var movingCamouflage = new DataItemViewDescriptor("MovingCamouflageValue", TankViewModelBase.MovingCamouflageDescriptor, 20);
            AddRelationship(movingCamouflage.Descriptor, CamouflageSkill.SkillDomain);
            camouflageValue.Items.Add(movingCamouflage);
            var firingStationaryCamouflage = new DataItemViewDescriptor("FiringStationaryCamouflageValue", TankViewModelBase.FiringStationaryCamouflageDescriptor, -30);
            AddRelationship(firingStationaryCamouflage.Descriptor, CamouflageSkill.SkillDomain);
            AddRelationship(firingStationaryCamouflage.Descriptor, "camouflageNet");
            AddRelationship(firingStationaryCamouflage.Descriptor, DetailedDataRelatedComponentType.Gun);
            camouflageValue.Items.Add(firingStationaryCamouflage);
            var firingMovingCamouflage = new DataItemViewDescriptor("FiringMovingCamouflageValue", TankViewModelBase.FiringMovingCamouflageDescriptor, -50);
            AddRelationship(firingMovingCamouflage.Descriptor, CamouflageSkill.SkillDomain);
            AddRelationship(firingMovingCamouflage.Descriptor, DetailedDataRelatedComponentType.Gun);
            camouflageValue.Items.Add(firingMovingCamouflage);

            scoutabilityViewDescriptor.Items.Add(camouflageValue);

            return new DataViewModel(scoutabilityViewDescriptor);
        }

        private DataViewModel CreateSurvivabilityDataGroup()
        {
            var survivabilityViewDescriptor = new DataGroupViewDescriptor("Survivability");

            var vehicleHealth = new ComplexDataItemViewDescriptor("Health");

            var tankHealth = new DataItemViewDescriptor("TankHealth", TankViewModelBase.HealthDescriptor, 50);
            AddRelationship(tankHealth.Descriptor, DetailedDataRelatedComponentType.Turret);
            vehicleHealth.Items.Add(tankHealth);

            var hullHealth = new DataItemViewDescriptor("HullHealth", HullViewModel.HealthDescriptor, -50);
            vehicleHealth.Items.Add(hullHealth);

            var turretHealth = new DataItemViewDescriptor("TurretHealth", TurretViewModel.HealthDescriptor, -30);
            AddRelationship(turretHealth.Descriptor, DetailedDataRelatedComponentType.Turret);
            vehicleHealth.Items.Add(turretHealth);

            survivabilityViewDescriptor.Items.Add(vehicleHealth);

            var componentHealth = new ComplexDataItemViewDescriptor("ComponentHealth");

            var ammoBayHealth = new DataItemViewDescriptor("AmmoBayHealth", AmmoBayViewModel.HealthDescriptor, 20);
            AddRelationship(ammoBayHealth.Descriptor, "staticFactorDevice:miscAttrs/ammoBayHealthFactor");
            AddRelationship(ammoBayHealth.Descriptor, PedantSkill.SkillDomain);
            componentHealth.Items.Add(ammoBayHealth);

            var turretRotatorHealth = new DataItemViewDescriptor("TurretRotatorHealth", TurretRotatorViewModel.HealthDescriptor, -60);
            AddRelationship(turretRotatorHealth.Descriptor, DetailedDataRelatedComponentType.Turret);
            componentHealth.Items.Add(turretRotatorHealth);

            var fuelTankHealth = new DataItemViewDescriptor("FuelTankHealth", FuelTankViewModel.HealthDescriptor, -40);
            AddRelationship(fuelTankHealth.Descriptor, "staticFactorDevice:miscAttrs/fuelTankHealthFactor");
            componentHealth.Items.Add(fuelTankHealth);

            var engineHealth = new DataItemViewDescriptor("EngineHealth", EngineViewModel.HealthDescriptor, -50);
            AddRelationship(engineHealth.Descriptor, DetailedDataRelatedComponentType.Engine);
            AddRelationship(engineHealth.Descriptor, "staticFactorDevice:miscAttrs/engineHealthFactor");
            componentHealth.Items.Add(engineHealth);

            var chassisHealth = new DataItemViewDescriptor("ChassisHealth", ChassisViewModel.HealthDescriptor, -20);
            AddRelationship(chassisHealth.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(chassisHealth.Descriptor, "enhancedSuspension");
            componentHealth.Items.Add(chassisHealth);

            var gunHealth = new DataItemViewDescriptor("GunHealth", GunViewModel.HealthDescriptor, -80);
            AddRelationship(gunHealth.Descriptor, DetailedDataRelatedComponentType.Gun);
            componentHealth.Items.Add(gunHealth);

            var surveyingDeviceHealth = new DataItemViewDescriptor("SurveyingDeviceHealth", SurveyingDeviceViewModel.HealthDescriptor, -120);
            AddRelationship(surveyingDeviceHealth.Descriptor, DetailedDataRelatedComponentType.Turret);
            componentHealth.Items.Add(surveyingDeviceHealth);

            var radioHealth = new DataItemViewDescriptor("RadioHealth", RadioViewModel.HealthDescriptor, -200);
            AddRelationship(radioHealth.Descriptor, DetailedDataRelatedComponentType.Radio);
            componentHealth.Items.Add(radioHealth);

            survivabilityViewDescriptor.Items.Add(componentHealth);

            var armor = new ComplexDataItemViewDescriptor("Armor");

            var hullFrontalArmor = new DataItemViewDescriptor("HullFrontalArmor", HullViewModel.FrontalArmorDescriptor, 20);
            armor.Items.Add(hullFrontalArmor);
            var hullSideArmor = new DataItemViewDescriptor("HullSideArmor", HullViewModel.SideArmorDescriptor, -50);
            armor.Items.Add(hullSideArmor);
            var hullRearArmor = new DataItemViewDescriptor("HullRearArmor", HullViewModel.RearArmorDescriptor, -120);
            armor.Items.Add(hullRearArmor);

            Predicate<TankViewModelBase> isArmorDefinedPredicate = t => t.LoadedModules.Turret.Turret.IsArmorDefined;

            var turretFrontalArmor = new DataItemViewDescriptor("TurretFrontalArmor", TurretViewModel.FrontalArmorDescriptor, 30, isArmorDefinedPredicate);
            AddRelationship(turretFrontalArmor.Descriptor, DetailedDataRelatedComponentType.Turret);
            armor.Items.Add(turretFrontalArmor);
            var turretSideArmor = new DataItemViewDescriptor("TurretSideArmor", TurretViewModel.SideArmorDescriptor, -40, isArmorDefinedPredicate);
            AddRelationship(turretSideArmor.Descriptor, DetailedDataRelatedComponentType.Turret);
            armor.Items.Add(turretSideArmor);
            var turretRearArmor = new DataItemViewDescriptor("TurretRearArmor", TurretViewModel.RearArmorDescriptor, -110, isArmorDefinedPredicate);
            AddRelationship(turretRearArmor.Descriptor, DetailedDataRelatedComponentType.Turret);
            armor.Items.Add(turretRearArmor);


            var chassisArmor = new DataItemViewDescriptor("ChassisArmor", ChassisViewModel.ArmorDescriptor, -70);
            AddRelationship(chassisArmor.Descriptor, DetailedDataRelatedComponentType.Chassis);
            armor.Items.Add(chassisArmor);

            var gunArmor = new DataItemViewDescriptor("GunArmor", GunViewModel.ArmorDescriptor, -130);
            AddRelationship(gunArmor.Descriptor, DetailedDataRelatedComponentType.Gun);
            armor.Items.Add(gunArmor);
            survivabilityViewDescriptor.Items.Add(armor);

            var engineFireChance = new DataItemViewDescriptor("EngineFireChance", EngineViewModel.FireChanceDescriptor, 10);
            AddRelationship(engineFireChance.Descriptor, DetailedDataRelatedComponentType.Engine);
            AddRelationship(engineFireChance.Descriptor, "extinguisher");
            AddRelationship(engineFireChance.Descriptor, TidyPersonSkill.SkillDomain);
            survivabilityViewDescriptor.Items.Add(engineFireChance);

            return new DataViewModel(survivabilityViewDescriptor);
        }

        private DataViewModel CreateMobilityDataGroup()
        {

            var mobilityViewDescriptor = new DataGroupViewDescriptor("Mobility");

            var enginePower = new DataItemViewDescriptor("EnginePower", EngineViewModel.EnginePowerDescriptor, 20);
            AddRelationship(enginePower.Descriptor, DetailedDataRelatedComponentType.Engine);
            AddRelationship(enginePower.Descriptor, "fuel");
            AddRelationship(enginePower.Descriptor, "removedRpmLimiter");
            mobilityViewDescriptor.Items.Add(enginePower);

            var specificPower = new DataItemViewDescriptor("SpecificPower", EngineViewModel.SpecificPowerDescriptor, 50);
            AddRelationship(specificPower.Descriptor, DetailedDataRelatedComponentType.Engine);
            AddRelationship(specificPower.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(specificPower.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(specificPower.Descriptor, DetailedDataRelatedComponentType.Radio);
            AddRelationship(specificPower.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(specificPower.Descriptor, DetailedDataRelatedComponentType.Equipment);
            AddRelationship(specificPower.Descriptor, "fuel");
            AddRelationship(specificPower.Descriptor, "removedRpmLimiter");
            mobilityViewDescriptor.Items.Add(specificPower);

            var chassisRotationSpeed = new DataItemViewDescriptor("ChassisRotationSpeed", ChassisViewModel.RotationSpeedDescriptor, 40);
            AddRelationship(chassisRotationSpeed.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(chassisRotationSpeed.Descriptor, VirtuosoSkill.SkillDomain);
            mobilityViewDescriptor.Items.Add(chassisRotationSpeed);

            var chassisSpeedLimits = new ComplexDataItemViewDescriptor("SpeedLimits");
            var forward = new DataItemViewDescriptor("Forward", ChassisViewModel.ForwardSpeedLimitDescriptor, 40);
            AddRelationship(forward.Descriptor, DetailedDataRelatedComponentType.Chassis);
            chassisSpeedLimits.Items.Add(forward);

            var backward = new DataItemViewDescriptor("Backward", ChassisViewModel.BackwardSpeedLimitDescriptor, -50);
            AddRelationship(backward.Descriptor, DetailedDataRelatedComponentType.Chassis);
            chassisSpeedLimits.Items.Add(backward);
            mobilityViewDescriptor.Items.Add(chassisSpeedLimits);

            var chassisTerrainResistances = new ComplexDataItemViewDescriptor("TerrainResistance");

            var onHardTerrain = new DataItemViewDescriptor("OnHardTerrain", ChassisViewModel.HardTerrainResistanceDescriptor, 50);
            AddRelationship(onHardTerrain.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(onHardTerrain.Descriptor, DriverSkill.SkillDomain);
            AddCrewSkillLevelRelationship(onHardTerrain.Descriptor);
            chassisTerrainResistances.Items.Add(onHardTerrain);

            var onMediumTerrain = new DataItemViewDescriptor("OnMediumTerrain", ChassisViewModel.MediumTerrainResistanceDescriptor, 50);
            AddRelationship(onMediumTerrain.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(onMediumTerrain.Descriptor, "grousers");
            AddRelationship(onMediumTerrain.Descriptor, DriverSkill.SkillDomain);
            AddRelationship(onMediumTerrain.Descriptor, BadRoadsKingSkill.SkillDomain);
            AddCrewSkillLevelRelationship(onMediumTerrain.Descriptor);
            chassisTerrainResistances.Items.Add(onMediumTerrain);

            var onSoftTerrain = new DataItemViewDescriptor("OnSoftTerrain", ChassisViewModel.SoftTerrainResistanceDescriptor, 50);
            AddRelationship(onSoftTerrain.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(onSoftTerrain.Descriptor, "grousers");
            AddRelationship(onSoftTerrain.Descriptor, DriverSkill.SkillDomain);
            AddRelationship(onSoftTerrain.Descriptor, BadRoadsKingSkill.SkillDomain);
            AddCrewSkillLevelRelationship(onSoftTerrain.Descriptor);
            chassisTerrainResistances.Items.Add(onSoftTerrain);
            mobilityViewDescriptor.Items.Add(chassisTerrainResistances);

            var chassisCanTraverse = new DataItemViewDescriptor("CanTraverse", ChassisViewModel.CanTraverseDescriptor, -80);
            AddRelationship(chassisCanTraverse.Descriptor, DetailedDataRelatedComponentType.Chassis);
            mobilityViewDescriptor.Items.Add(chassisCanTraverse);

            var chassisBrakeForce = new DataItemViewDescriptor("BrakeForce", ChassisViewModel.BrakeForceDescriptor, -190);
            AddRelationship(chassisBrakeForce.Descriptor, DetailedDataRelatedComponentType.Chassis);
            mobilityViewDescriptor.Items.Add(chassisBrakeForce);

            var chassisMaxClimbAngle = new DataItemViewDescriptor("MaxClimbAngle", ChassisViewModel.MaxClimbAngleDescriptor, -170);
            AddRelationship(chassisMaxClimbAngle.Descriptor, DetailedDataRelatedComponentType.Chassis);
            mobilityViewDescriptor.Items.Add(chassisMaxClimbAngle);

            return new DataViewModel(mobilityViewDescriptor);
        }

        private DataViewModel CreateManeuverabilityDataGroup()
        {

            var maneuverabilityViewDescriptor = new DataGroupViewDescriptor("Maneuverability");

            var gunAccuracy = new DataItemViewDescriptor("Accuracy", GunViewModel.AccuracyDescriptor, 100);
            AddRelationship(gunAccuracy.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunAccuracy.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(gunAccuracy.Descriptor, GunnerSkill.SkillDomain);
            AddCrewSkillLevelRelationship(gunAccuracy.Descriptor);
            maneuverabilityViewDescriptor.Items.Add(gunAccuracy);

            var aimingTime = new DataItemViewDescriptor("AimingTime", GunViewModel.AimingTimeDescriptor, 90);
            AddRelationship(aimingTime.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(aimingTime.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(aimingTime.Descriptor, "staticFactorDevice:miscAttrs/gunAimingTimeFactor");
            AddRelationship(aimingTime.Descriptor, GunnerSkill.SkillDomain);
            AddCrewSkillLevelRelationship(aimingTime.Descriptor);
            maneuverabilityViewDescriptor.Items.Add(aimingTime);

            var dispersion = new ComplexDataItemViewDescriptor("GunShotDispersion");

            var gunDispersionAfterShot = new DataItemViewDescriptor("DispersionAfterShot", GunViewModel.DispersionAfterShotDescriptor, 60);
            AddRelationship(gunDispersionAfterShot.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunDispersionAfterShot.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(gunDispersionAfterShot.Descriptor, "staticFactorDevice:miscAttrs/additiveShotDispersionFactor");
            AddRelationship(gunDispersionAfterShot.Descriptor, GunnerSkill.SkillDomain);
            AddCrewSkillLevelRelationship(gunDispersionAfterShot.Descriptor);
            dispersion.Items.Add(gunDispersionAfterShot);

            var gunDispersionOnTurretRotation = new DataItemViewDescriptor("DispersionOnTurretRotation", GunViewModel.DispersionOnTurretRotationDescriptor, 50);
            AddRelationship(gunDispersionOnTurretRotation.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunDispersionOnTurretRotation.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(gunDispersionOnTurretRotation.Descriptor, "staticFactorDevice:miscAttrs/additiveShotDispersionFactor");
            AddRelationship(gunDispersionOnTurretRotation.Descriptor, GunnerSkill.SkillDomain);
            AddRelationship(gunDispersionOnTurretRotation.Descriptor, SmoothTurretSkill.SkillDomain);
            AddCrewSkillLevelRelationship(gunDispersionOnTurretRotation.Descriptor);
            dispersion.Items.Add(gunDispersionOnTurretRotation);


            var chassisDispersionOnVehicleMove = new DataItemViewDescriptor("DispersionOnVehicleMove", ChassisViewModel.DispersionOnVehicleMoveDescriptor, 55);
            AddRelationship(chassisDispersionOnVehicleMove.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(chassisDispersionOnVehicleMove.Descriptor, "staticFactorDevice:miscAttrs/additiveShotDispersionFactor");
            AddRelationship(chassisDispersionOnVehicleMove.Descriptor, GunnerSkill.SkillDomain);
            AddRelationship(chassisDispersionOnVehicleMove.Descriptor, SmoothDrivingSkill.SkillDomain);
            AddCrewSkillLevelRelationship(chassisDispersionOnVehicleMove.Descriptor);
            dispersion.Items.Add(chassisDispersionOnVehicleMove);

            var chassisDispersionOnVehicleRotation = new DataItemViewDescriptor("DispersionOnVehicleRotation", ChassisViewModel.DispersionOnVehicleRotationDescriptor, 50);
            AddRelationship(chassisDispersionOnVehicleRotation.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(chassisDispersionOnVehicleRotation.Descriptor, "staticFactorDevice:miscAttrs/additiveShotDispersionFactor");
            AddRelationship(chassisDispersionOnVehicleRotation.Descriptor, GunnerSkill.SkillDomain);
            AddCrewSkillLevelRelationship(chassisDispersionOnVehicleRotation.Descriptor);
            dispersion.Items.Add(chassisDispersionOnVehicleRotation);

            var gunDispersionOnTurretFullSpeedRotating = new DataItemViewDescriptor("DispersionOnTurretFullSpeedRotating", GunViewModel.DispersionOnTurretFullSpeedRotatingDescriptor, 60);
            AddRelationship(gunDispersionOnTurretFullSpeedRotating.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunDispersionOnTurretFullSpeedRotating.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(gunDispersionOnTurretFullSpeedRotating.Descriptor, "staticFactorDevice:miscAttrs/additiveShotDispersionFactor");
            AddRelationship(gunDispersionOnTurretFullSpeedRotating.Descriptor, GunnerSkill.SkillDomain);
            AddRelationship(gunDispersionOnTurretFullSpeedRotating.Descriptor, SmoothTurretSkill.SkillDomain);
            AddCrewSkillLevelRelationship(gunDispersionOnTurretFullSpeedRotating.Descriptor);
            dispersion.Items.Add(gunDispersionOnTurretFullSpeedRotating);

            var chassisDispersionOnVehicleFullSpeedMoving = new DataItemViewDescriptor("DispersionOnVehicleFullSpeedMoving", ChassisViewModel.DispersionOnVehicleFullSpeedMovingDescriptor, 65);
            AddRelationship(chassisDispersionOnVehicleFullSpeedMoving.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(chassisDispersionOnVehicleFullSpeedMoving.Descriptor, "staticFactorDevice:miscAttrs/additiveShotDispersionFactor");
            AddRelationship(chassisDispersionOnVehicleFullSpeedMoving.Descriptor, GunnerSkill.SkillDomain);
            AddRelationship(chassisDispersionOnVehicleFullSpeedMoving.Descriptor, SmoothDrivingSkill.SkillDomain);
            AddCrewSkillLevelRelationship(chassisDispersionOnVehicleFullSpeedMoving.Descriptor);
            dispersion.Items.Add(chassisDispersionOnVehicleFullSpeedMoving);


            var chassisDispersionOnVehicleFullSpeedRotating = new DataItemViewDescriptor("DispersionOnVehicleFullSpeedRotating", ChassisViewModel.DispersionOnVehicleFullSpeedRotatingDescriptor, 60);
            AddRelationship(chassisDispersionOnVehicleFullSpeedRotating.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(chassisDispersionOnVehicleFullSpeedRotating.Descriptor, "staticFactorDevice:miscAttrs/additiveShotDispersionFactor");
            AddRelationship(chassisDispersionOnVehicleFullSpeedRotating.Descriptor, GunnerSkill.SkillDomain);
            AddCrewSkillLevelRelationship(chassisDispersionOnVehicleFullSpeedRotating.Descriptor);
            dispersion.Items.Add(chassisDispersionOnVehicleFullSpeedRotating);

            var dispersionWhileGunDamaged = new DataItemViewDescriptor("DispersionWhileGunDamaged", GunViewModel.DispersionWhileGunDamagedDescriptor, -150);
            AddRelationship(dispersionWhileGunDamaged.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(dispersionWhileGunDamaged.Descriptor, GunsmithSkill.SkillDomain);
            dispersion.Items.Add(dispersionWhileGunDamaged);
            maneuverabilityViewDescriptor.Items.Add(dispersion);

            var gunElevation = new DataItemViewDescriptor("Elevation", GunViewModel.ElevationDescriptor, -50);
            AddRelationship(gunElevation.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunElevation.Descriptor, DetailedDataRelatedComponentType.Turret);
            maneuverabilityViewDescriptor.Items.Add(gunElevation);

            var gunDepression = new DataItemViewDescriptor("Depression", GunViewModel.DepressionDescriptor, 70);
            AddRelationship(gunDepression.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunDepression.Descriptor, DetailedDataRelatedComponentType.Turret);
            maneuverabilityViewDescriptor.Items.Add(gunDepression);

	        bool IsGunHorizontalTraverseLimitedPredicate(TankViewModelBase tank) => tank.LoadedModules.Gun.IsHorizontalTraverseLimited;

	        var gunHorizontalTraverse = new DataItemViewDescriptor("GunHorizontalTraverse", GunViewModel.HorizontalTraverseDescriptor, 30, IsGunHorizontalTraverseLimitedPredicate);
            AddRelationship(gunHorizontalTraverse.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunHorizontalTraverse.Descriptor, DetailedDataRelatedComponentType.Turret);
            maneuverabilityViewDescriptor.Items.Add(gunHorizontalTraverse);


            var gunRotationSpeed = new DataItemViewDescriptor("GunRotationSpeed", GunViewModel.RotationSpeedDescriptor, -120);
            AddRelationship(gunRotationSpeed.Descriptor, DetailedDataRelatedComponentType.Gun);
            maneuverabilityViewDescriptor.Items.Add(gunRotationSpeed);

            var turretRotationSpeed = new DataItemViewDescriptor("TurretRotationSpeed", TurretViewModel.RotationSpeedDescriptor, 30);
            AddRelationship(turretRotationSpeed.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(turretRotationSpeed.Descriptor, "fuel");
            AddRelationship(turretRotationSpeed.Descriptor, GunnerSkill.SkillDomain);
            maneuverabilityViewDescriptor.Items.Add(turretRotationSpeed);

            var chassisRotationSpeed = new DataItemViewDescriptor("ChassisRotationSpeed", ChassisViewModel.RotationSpeedDescriptor, 30);
            AddRelationship(chassisRotationSpeed.Descriptor, DetailedDataRelatedComponentType.Chassis);
            AddRelationship(chassisRotationSpeed.Descriptor, VirtuosoSkill.SkillDomain);
            maneuverabilityViewDescriptor.Items.Add(chassisRotationSpeed);

            return new DataViewModel(maneuverabilityViewDescriptor);
        }

        private DataViewModel CreateFirePowerDataGroup()
        {

            var firePowerViewDescriptor = new DataGroupViewDescriptor("FirePower");

            var shellPenetration = new DataItemViewDescriptor("Penetration", ShellViewModel.PenetrationDescriptor, 100);
            AddRelationship(shellPenetration.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellPenetration.Descriptor, DetailedDataRelatedComponentType.Gun);

            AddRelationship(shellPenetration.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellPenetration);

            var shellDamage = new DataItemViewDescriptor("Damage", ShellViewModel.ArmorDamageDescriptor, 50);
            AddRelationship(shellDamage.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellDamage.Descriptor, DetailedDataRelatedComponentType.Gun);

            AddRelationship(shellDamage.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellDamage);

            var dpm = new DataItemViewDescriptor("DamagePerMinute", ShellViewModel.DamagePerMinuteDescriptor, 60);
            AddRelationship(dpm.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(dpm.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(dpm.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(dpm.Descriptor, "staticFactorDevice:miscAttrs/gunReloadTimeFactor");
            AddRelationship(dpm.Descriptor, LoaderSkill.SkillDomain);
            AddCrewSkillLevelRelationship(dpm.Descriptor);
            firePowerViewDescriptor.Items.Add(dpm);

            var shellModuleDamage = new DataItemViewDescriptor("ModuleDamage", ShellViewModel.ModuleDamageDescriptor, -50);
            AddRelationship(shellModuleDamage.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellModuleDamage.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(shellModuleDamage.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellModuleDamage);

            var gunReloadTime = new DataItemViewDescriptor("ReloadTime", GunViewModel.ReloadTimeDescriptor, 30);
            AddRelationship(gunReloadTime.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunReloadTime.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(gunReloadTime.Descriptor, "staticFactorDevice:miscAttrs/gunReloadTimeFactor");
            AddRelationship(gunReloadTime.Descriptor, LoaderSkill.SkillDomain);
            AddCrewSkillLevelRelationship(gunReloadTime.Descriptor);
            firePowerViewDescriptor.Items.Add(gunReloadTime);

            var gunRateOfFire = new DataItemViewDescriptor("RateOfFire", GunViewModel.RateOfFireDescriptor, -30);
            AddRelationship(gunRateOfFire.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunRateOfFire.Descriptor, DetailedDataRelatedComponentType.Turret);
            AddRelationship(gunRateOfFire.Descriptor, "staticFactorDevice:miscAttrs/gunReloadTimeFactor");
            AddRelationship(gunRateOfFire.Descriptor, LoaderSkill.SkillDomain);
            AddCrewSkillLevelRelationship(gunRateOfFire.Descriptor);
            firePowerViewDescriptor.Items.Add(gunRateOfFire);

            var gunMaxAmmo = new DataItemViewDescriptor("MaxAmmo", GunViewModel.MaxAmmoDescriptor, -150);
            AddRelationship(gunMaxAmmo.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunMaxAmmo.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(gunMaxAmmo);

            Predicate<TankViewModelBase> isMagazineGunPredicate = tank => tank.LoadedModules.Gun.IsMagazineGun;

            var gunClipCount = new DataItemViewDescriptor("ClipCount", GunViewModel.ClipCountDescriptor, 30, isMagazineGunPredicate);
            AddRelationship(gunClipCount.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunClipCount.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(gunClipCount);

            var gunClipReloadTime = new DataItemViewDescriptor("ClipReloadTime", GunViewModel.ClipReloadTimeDescriptor, 50, isMagazineGunPredicate);
            AddRelationship(gunClipReloadTime.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunClipReloadTime.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(gunClipReloadTime);

            Predicate<TankViewModelBase> hasBurstPredicate = tank => tank.LoadedModules.Gun.HasBurst;

            var gunBurstCount = new DataItemViewDescriptor("BurstCount", GunViewModel.BurstCountDescriptor, -50, hasBurstPredicate);
            AddRelationship(gunBurstCount.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunBurstCount.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(gunBurstCount);

            var gunBurstRateOfFire = new DataItemViewDescriptor("BurstRateOfFire", GunViewModel.BurstRateOfFireDescriptor, -70, hasBurstPredicate);
            AddRelationship(gunBurstRateOfFire.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunBurstRateOfFire.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(gunBurstRateOfFire);


            Predicate<TankViewModelBase> hasExplosionRadiusPredict = tank => tank.LoadedModules.Gun.SelectedShell.HasExplosionRadius;

            var shellExplosionRadius = new DataItemViewDescriptor("ExplosionRadius", ShellViewModel.ExplosionRadiusDescriptor, showCondition: hasExplosionRadiusPredict);

            shellExplosionRadius.PrioritySelector = tank => tank.Tank.Class == TankClass.SelfPropelledGun ? 10 : -10;

            AddRelationship(shellExplosionRadius.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellExplosionRadius.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(shellExplosionRadius.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellExplosionRadius);


            Predicate<TankViewModelBase> hasPiercingPowerLossFactor = tank => tank.LoadedModules.Gun.SelectedShell.HasPiercingPowerLossFactor;

            var shellPenetrationLossFactorByDistance = new DataItemViewDescriptor("PenetrationLossFactorByDistance", ShellViewModel.PiercingPowerLossFactorByDistanceDescriptor, -120, hasPiercingPowerLossFactor);
            AddRelationship(shellPenetrationLossFactorByDistance.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellPenetrationLossFactorByDistance.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(shellPenetrationLossFactorByDistance.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellPenetrationLossFactorByDistance);


            var gunCaliber = new DataItemViewDescriptor("Caliber", GunViewModel.CaliberDescriptor, -140);
            AddRelationship(gunCaliber.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(gunCaliber.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(gunCaliber);

            var shellMaxDistance = new DataItemViewDescriptor("MaxDistance", ShellViewModel.MaxDistanceDescriptor, -130);
            AddRelationship(shellMaxDistance.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellMaxDistance.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(shellMaxDistance.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellMaxDistance);

            var shellSpeed = new DataItemViewDescriptor("Speed", ShellViewModel.SpeedDescriptor, -80);
            AddRelationship(shellSpeed.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellSpeed.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(shellSpeed.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellSpeed);

            var shellGravity = new DataItemViewDescriptor("Gravity", ShellViewModel.GravityDescriptor);
            shellGravity.PrioritySelector = tank => tank.Tank.Class == TankClass.SelfPropelledGun ? -30 : -130;

            AddRelationship(shellGravity.Descriptor, DetailedDataRelatedComponentType.Shell);
            AddRelationship(shellGravity.Descriptor, DetailedDataRelatedComponentType.Gun);
            AddRelationship(shellGravity.Descriptor, DetailedDataRelatedComponentType.Turret);
            firePowerViewDescriptor.Items.Add(shellGravity);

            return new DataViewModel(firePowerViewDescriptor);
        }

        private void InitializeDataGroups()
        {
            _dataItemDescriptorComponentTypeRelationships = new Dictionary<DataItemDescriptorBase, Collection<DetailedDataRelatedComponentType>>();
            _dataItemDescriptorModificationDomainRelationships = new Dictionary<DataItemDescriptorBase, Collection<string>>();

            this.FirePowerDataGroup = this.CreateFirePowerDataGroup();
            this.ManeuverabilityDataGroup = this.CreateManeuverabilityDataGroup();
            this.MobilityDataGroup = this.CreateMobilityDataGroup();
            this.SurvivabilityDataGroup = this.CreateSurvivabilityDataGroup();
            this.ScoutabilityDataGroup = this.CreateScoutabilityDataGroup();
            this.EconomyDataGroup = this.CreateEconomyDataGroup();
            this.BattleDataGroup = this.CreateBattleDataGroup();
            this.MiscellaneousDataGroup = this.CreateMiscellaneousDataGroup();

            _allDataViewModels = new[]
                    {
                        this.FirePowerDataGroup,
                        this.ManeuverabilityDataGroup,
                        this.MobilityDataGroup,
                        this.SurvivabilityDataGroup,
                        this.ScoutabilityDataGroup,
                        this.EconomyDataGroup,
                        this.BattleDataGroup,
                        this.MiscellaneousDataGroup
                    };

            this.RemapDataItems();
        }

        private void RemapDataItems()
        {
            _allDataItems = new List<DataItem>();
            _allComplexDataItemVMs = new List<ComplexDataItemViewModel>();

            _dataItemComponentRelationships = new Dictionary<DetailedDataRelatedComponentType, List<DataItem>>();
            _dataItemModificationDomainRelationships = new Dictionary<string, List<DataItem>>();
            foreach (var dataVm in _allDataViewModels)
            {
                this.MapDataItems(dataVm.DataGroup.Items);
            }
        }

        private void MapDataItems(IEnumerable<DataItemViewModelBase> items)
        {
            foreach (var item in items)
            {
                var complexItem = item as ComplexDataItemViewModel;
                if (complexItem != null)
                {
                    _allComplexDataItemVMs.Add(complexItem);
                    this.MapDataItems(complexItem.Items);
                    continue;
                }

                var dataItem = item as DataItemViewModel;
                if (dataItem != null)
                {
					if (_dataItemDescriptorComponentTypeRelationships.TryGetValue(dataItem.DataItem.Descriptor, out Collection<DetailedDataRelatedComponentType> componentTypes))
						foreach (var componentType in componentTypes)
						{
							var dataItems = _dataItemComponentRelationships.GetOrCreate(componentType, () => new List<DataItem>());
							dataItems.Add(dataItem.DataItem);
						}

					if (_dataItemDescriptorModificationDomainRelationships.TryGetValue(dataItem.DataItem.Descriptor, out Collection<string> modificationDomains))
						foreach (var domain in modificationDomains)
						{
							var dataItems = _dataItemModificationDomainRelationships.GetOrCreate(domain, () => new List<DataItem>());
							dataItems.Add(dataItem.DataItem);
						}

					_allDataItems.Add(dataItem.DataItem);
                }
            }
        }

    }
}
