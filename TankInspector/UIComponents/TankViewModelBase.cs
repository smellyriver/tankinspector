using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using log4net;
using System.Reflection;
using Smellyriver.TankInspector.Design;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract partial class TankViewModelBase : TankObjectViewModel, ICloneable
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        public virtual int RepairCost
        {
            get
            {
                var price = (this.Tank.Hull.MaxHealth + this.LoadedModules.Turret.Turret.MaxHealth) * this.Tank.RepairCostFactor;
                foreach (var module in this.LoadedModules.Select(vm => vm.Module)
                                                         .OfType<IDamageableModule>())
                    price += module.MaxHealth * module.RepairCostFactor;

                return (int)price;
            }
        }

        public new PriceViewModel Price { get; protected set; }

        public virtual double Health => this.LoadedModules.Turret.Turret.MaxHealth + this.LoadedModules.Hull.Hull.MaxHealth;

	    public double ModuleWeight
        {
            get { return this.LoadedModules.Sum(m => m.Weight); }
        }

        public double Weight => this.ModuleWeight + this.EquipmentWeight;

	    public double EquipmentWeight
        {
            get { return this.LoadedEquipments.Sum(m => m.GetWeight(this.ModuleWeight)); }
        }

        public abstract string IconKey { get; }
        public abstract string NationKey { get; }
        public BattleTiersViewModel BattleTiers { get; }
        public abstract IEnumerable<TankViewModelBase> Predecessors { get; }
        public abstract IEnumerable<TankViewModelBase> Successors { get; }
        public abstract bool HasSuccessor { get; }
        public abstract bool HasPredecessor { get; }
        public bool IsElite
        {
            get { return this.LoadedModules.OfType<IChangableComponent>().All(module => module.IsElite); }
        }

        public string TaxologyDescription
        {
            get
            {
                var formatString = this.IsElite
                    ? App.GetLocalizedString("EliteTankTaxologyDescription")
                    : App.GetLocalizedString("TankTaxologyDescription");
                return string.Format(formatString,
                    RomanNumberService.GetRomanNumber(this.Tier),
                    App.GetLocalizedString(this.Tank.Class.ToString()));
            }
        }

        public bool IsTdorSPG => this.Tank.Class == TankClass.TankDestroyer || this.Tank.Class == TankClass.SelfPropelledGun;

	    private void GetStationaryCamoflagueValues(out double? normalValue, out double? withCamoNet, out double? secondsBeforeCamoNetActivation)
        {
            var hasInvisibilityData = this.Tank.Invisibility != null;

            if (!hasInvisibilityData && this.Tank.CamouflageValue == null)
            {
                normalValue = null;
                withCamoNet = null;
                secondsBeforeCamoNetActivation = null;
            }
            else
            {
                var value = hasInvisibilityData ? this.Tank.Invisibility.Still : this.Tank.CamouflageValue.Stationary;
                var camoSkillFactor = this.ModificationContext.GetValue(CamouflageSkill.SkillDomain, CamouflageSkill.CamouflageFactorSkillKey, 1.0);
                normalValue = value * camoSkillFactor;
                if (this.ModificationContext.HasDomain("camouflageNet"))
                {
                    secondsBeforeCamoNetActivation = this.ModificationContext.GetValue("camouflageNet", "activateWhenStillSec", 3.0);
                    var camoNetBonus = hasInvisibilityData
                                           ? this.Tank.Invisibility.CamouflageNetBonus
                                           : CamouflageNetScript.AdditiveCamouflageValues[this.Tank.Class];
                    withCamoNet = normalValue + camoNetBonus;
                }
                else
                {
                    secondsBeforeCamoNetActivation = null;
                    withCamoNet = null;
                }
            }
        }

        public CamouflageValueComponentViewModel StationaryCamoflagueValue
        {
            get
            {
				this.GetStationaryCamoflagueValues(out double? normalValue, out double? withCamoNet, out double? secondsBeforeCamoNetActivation);

                if (normalValue == null)
                    return CamouflageValueComponentViewModel.NotApplicable;
	            if (withCamoNet != null)
		            return new CamouflageValueComponentWithCamouflageNetViewModel(withCamoNet.Value, normalValue.Value, secondsBeforeCamoNetActivation.Value);
	            return new CamouflageValueComponentViewModel(normalValue.Value);
            }
        }

        private double? GetMovingCamoflagueValue()
        {
            var hasInvisibilityData = this.Tank.Invisibility != null;

            if (!hasInvisibilityData && this.Tank.CamouflageValue == null)
                return null;
	        var value = hasInvisibilityData ? this.Tank.Invisibility.Moving : this.Tank.CamouflageValue.Moving;
	        var camoSkillFactor = this.ModificationContext.GetValue(CamouflageSkill.SkillDomain, CamouflageSkill.CamouflageFactorSkillKey, 0.5);
	        return value * camoSkillFactor;
        }

        public CamouflageValueComponentViewModel MovingCamoflagueValue
        {
            get
            {
                var value = this.GetMovingCamoflagueValue();
                if (value == null)
                    return CamouflageValueComponentViewModel.NotApplicable;
	            return new CamouflageValueComponentViewModel(value);
            }
        }

        private double GetCamouflageFactorAfterShot(IGun gun)
        {

            //gun.CamouflageFactorAfterShot is removed in 8.11CT

            if (this.Tank.Database.VersionId <= 810)
                return gun.CamouflageFactorAfterShot;

            if (this.Tank.Invisibility != null)
                return Math.Max(0, 1 - (this.Tank.Invisibility.FirePenalty / this.Tank.Invisibility.Still));

            if (this.Tank.CamouflageValue == null || this.Tank.CamouflageValue.Stationary == 0)
                return 0;

            return this.Tank.CamouflageValue.Firing / this.Tank.CamouflageValue.Stationary;

        }

        public CamouflageValueComponentViewModel FiringStationaryCamoflagueValue
        {
            get
            {
				this.GetStationaryCamoflagueValues(out double? normalValue, out double? withCamoNet, out double? secondsBeforeCamoNetActivation);

                if (normalValue == null)
                    return CamouflageValueComponentViewModel.NotApplicable;
	            var factor = this.GetCamouflageFactorAfterShot(this.LoadedModules.Gun.Gun);

	            if (withCamoNet != null)
		            return new CamouflageValueComponentWithCamouflageNetViewModel(withCamoNet.Value * factor, normalValue.Value * factor, secondsBeforeCamoNetActivation.Value);
	            return new CamouflageValueComponentViewModel(normalValue.Value * factor);
            }
        }

        public CamouflageValueComponentViewModel FiringMovingCamoflagueValue
        {
            get
            {
                var value = this.GetMovingCamoflagueValue();
                if (value == null)
                    return CamouflageValueComponentViewModel.NotApplicable;
	            var factor = this.GetCamouflageFactorAfterShot(this.LoadedModules.Gun.Gun);
	            return new CamouflageValueComponentViewModel(value * factor);
            }
        }


        public ITank Tank => (ITank)this.TankObject;

	    public HangarViewModel Owner { get; }

	    protected TankViewModelBase(CommandBindingCollection commandBindings, ITank tank, HangarViewModel owner)
            : base(commandBindings, tank)
        {
            this.Owner = owner;

            this.BattleTiers = new BattleTiersViewModel(this.Tank.BattleTiers);
            this.Price = new PriceViewModel(this.Tank.Price, this.Tank.CurrencyType);

            this.InitializeModuleViewModels();

            this.InitializeEquipmentAndConsumableViewModels();

            this.InitializeCrews();

            this.UpdateModificationContext();
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public virtual TankViewModelBase Clone(TankViewModelCloneFlags flags = TankViewModelCloneFlags.All)
        {
            var clone = (TankViewModelBase)this.MemberwiseClone();

            var cloneModules = (flags & TankViewModelCloneFlags.AnyModule) == TankViewModelCloneFlags.AnyModule;
            if (cloneModules)
            {
                clone.LoadedModules = (ModuleViewModels)this.LoadedModules.Clone(clone, flags);
                clone.AvailableGuns = new Dictionary<IGun, GunViewModel>(this.AvailableGuns);
            }

            var cloneEquipments = (flags & TankViewModelCloneFlags.Equipments) == TankViewModelCloneFlags.Equipments;
            if (cloneEquipments)
            {
                clone.LoadedEquipments = new ObservableCollection<IEquipmentViewModel>(this.LoadedEquipments);
                clone.HandleEquipmentsChangeEvents();
            }

            var cloneConsumables = (flags & TankViewModelCloneFlags.Consumables) == TankViewModelCloneFlags.Consumables;
            if (cloneConsumables)
            {
                clone.LoadedConsumables = new ObservableCollection<IConsumableViewModel>(this.LoadedConsumables);
                clone.HandleConsumablesChangeEvents();
            }

            var cloneCrews = (flags & TankViewModelCloneFlags.AnyCrew) == TankViewModelCloneFlags.AnyCrew;
            if (cloneCrews)
            {
                clone.Crews = clone.Crews.Clone(clone, flags);
                clone.HandleCrewSkillsChangedEvent();   // fixme: potential event memory leak, weak event required
            }

            if (cloneEquipments || cloneCrews)
            {
                clone.ModificationContext = this.ModificationContext.Clone();
            }

            clone.ClearEventHandlers();
            return clone;
        }
    }
}
