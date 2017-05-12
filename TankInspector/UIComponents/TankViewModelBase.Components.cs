using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class TankViewModelBase
    {

        public const int MaxEquipmentCount = 3;
        public const int MaxConsumableCount = 3;

        public ModuleViewModels LoadedModules { get; private set; }

        public ObservableCollection<IEquipmentViewModel> LoadedEquipments { get; private set; }

        internal int? CurrentEquipmentSlotIndex { get; set; }

        internal IEquipmentViewModel CurrentEquipment
        {
            get
            {
	            if (this.CurrentEquipmentSlotIndex.HasValue)
                    return this.LoadedEquipments[this.CurrentEquipmentSlotIndex.Value];
	            return null;
            }
        }

        public ObservableCollection<IConsumableViewModel> LoadedConsumables { get; private set; }

        internal IConsumableViewModel CurrentConsumable
        {
            get
            {
	            if (this.CurrentConsumableSlotIndex.HasValue)
                    return this.LoadedConsumables[this.CurrentConsumableSlotIndex.Value];
	            return null;
            }
        }

        internal int? CurrentConsumableSlotIndex { get; set; }

        // stores modification effects affected by equipments, consumables and crew skills
        internal ModificationContext ModificationContext { get; private set; }


        public Dictionary<IChassis, ChassisViewModel> AvailableChassis { get; private set; }

        private Dictionary<IGun, GunViewModel> _gunViewModelCache;

        // only guns available for current turret is listed here. to access all the gun view models, use _gunViewModelCache
        public Dictionary<IGun, GunViewModel> AvailableGuns { get; private set; }
        public Dictionary<ITurret, TurretViewModel> AvailableTurrets { get; private set; }
        public Dictionary<IRadio, RadioViewModel> AvailableRadios { get; private set; }
        public Dictionary<IEngine, EngineViewModel> AvailableEngines { get; private set; }
        public IEnumerable<ModuleViewModel> AllChangableModules => ((IEnumerable<ModuleViewModel>)this.AvailableChassis.Values)
	        .Union((IEnumerable<ModuleViewModel>)this.AvailableGuns.Values)
	        .Union((IEnumerable<ModuleViewModel>)this.AvailableTurrets.Values)
	        .Union((IEnumerable<ModuleViewModel>)this.AvailableRadios.Values)
	        .Union((IEnumerable<ModuleViewModel>)this.AvailableEngines.Values);

	    public Dictionary<Equipment, IEquipmentViewModel> AvailableEquipments { get; private set; }
        public Dictionary<Consumable, IConsumableViewModel> AvailableConsumables { get; private set; }

        private IEquipmentViewModel _removeEquipmentViewModel;
        private IConsumableViewModel _removeConsumableViewModel;
        private IEquipmentViewModel _emptyEquipmentViewModel;
        private IConsumableViewModel _emptyConsumableViewModel;

        internal IEnumerable<IEquipmentViewModel> EquipmentReplacements => (new[] { _removeEquipmentViewModel }).Union(this.AvailableEquipments.Values);

	    internal IEnumerable<IConsumableViewModel> ConsumableReplacements => (new[] { _removeConsumableViewModel }).Union(this.AvailableConsumables.Values);

	    public CrewViewModelCollection Crews { get; private set; }

        public HullViewModel Hull { get; private set; }
        public FuelTankViewModel FuelTank { get; private set; }


        private int _changeModulesProcedureCount = 0;

        private bool _suppressModificationContextUpdate = false;


        private void InitializeCrews()
        {
            this.Crews = new CrewViewModelCollection(this.CommandBindings);

            for (int i = 0; i < this.Tank.Crews.Count; ++i)
            {
                this.Crews.Add(new CrewViewModel(this.CommandBindings, this.Tank.Crews[i], this, i));
            }

            this.HandleCrewSkillsChangedEvent();
        }

        private void HandleCrewSkillsChangedEvent()
        {
            foreach (var crew in this.Crews)
                crew.EffectiveSkillsChanged += Crews_EffectiveSkillsChanged;
        }

	    private void Crews_EffectiveSkillsChanged(object sender, EventArgs e)
        {
            this.UpdateModificationContext();
        }


        private void InitializeEquipmentAndConsumableViewModels()
        {
            this.ModificationContext = new ModificationContext();
            this.LoadedEquipments = new ObservableCollection<IEquipmentViewModel>();
            this.LoadedConsumables = new ObservableCollection<IConsumableViewModel>();

            this.AvailableEquipments = this.Tank.AvailableEquipments.ToDictionary(p => p.Value, p => (IEquipmentViewModel)new EquipmentViewModel(this.CommandBindings, p.Value, this));
            this.AvailableConsumables = this.Tank.AvailableConsumables.ToDictionary(p => p.Value, p => (IConsumableViewModel)new ConsumableViewModel(this.CommandBindings, p.Value, this));

            _removeEquipmentViewModel = new RemoveEquipmentViewModel(this.CommandBindings, this);
            _removeConsumableViewModel = new RemoveConsumableViewModel(this.CommandBindings, this);
            _emptyEquipmentViewModel = new EmptyEquipmentViewModel(this.CommandBindings, this);
            _emptyConsumableViewModel = new EmptyConsumableViewModel(this.CommandBindings, this);

            for (int i = 0; i < MaxEquipmentCount; ++i)
                this.LoadedEquipments.Add(_emptyEquipmentViewModel);

            for (int i = 0; i < MaxConsumableCount; ++i)
                this.LoadedConsumables.Add(_emptyConsumableViewModel);

            this.HandleEquipmentsChangeEvents();
            this.HandleConsumablesChangeEvents();

        }

        private void HandleEquipmentsChangeEvents()
        {
            this.LoadedEquipments.CollectionChanged += AccessoryCollectionChanged;
        }

        private void HandleConsumablesChangeEvents()
        {
            this.LoadedConsumables.CollectionChanged += AccessoryCollectionChanged;
        }

	    private void AccessoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateModificationContext();
        }

        private void UpdateModificationContext()
        {
            if (_suppressModificationContextUpdate)
                return;

            _suppressModificationContextUpdate = true;
            this.ModificationContext.BeginInit();

            this.ModificationContext.Clear();

            // caution: keep this sequence
            foreach (var equipment in this.LoadedEquipments.OfType<EquipmentViewModel>())
                equipment.Equipment.Script.Execute(this.ModificationContext, null);

            foreach (var consumable in this.LoadedConsumables.OfType<ConsumableViewModel>())
                consumable.Consumable.Script.Execute(this.ModificationContext, null);

            var crewSkills = this.Crews.SelectMany(
                c => ((IEnumerable<CrewSkillViewModelBase>)c.BasicSkills)
                    .Union(c.AvailableSkills.Values))
                .OrderBy(s => s.CrewSkill.Priority);

            // commander first
            foreach (var skill in crewSkills)
            {
                skill.CrewSkill.Execute(this.ModificationContext, skill.Owner.GetSkillLevel(skill));
            }

            this.ModificationContext.EndInit();
            _suppressModificationContextUpdate = false;
        }

        private void InitializeModuleViewModels()
        {
            this.LoadedModules = new ModuleViewModels();

            this.Hull = new HullViewModel(this.CommandBindings, this.Tank.Hull, this);
            this.FuelTank = new FuelTankViewModel(this.CommandBindings, this.Tank.AvailableFuelTanks.Values.FirstOrDefault(), this);

            this.LoadedModules.Hull = this.Hull;
            this.LoadedModules.FuelTank = this.FuelTank;

            this.AvailableChassis = new Dictionary<IChassis, ChassisViewModel>();
            this.AvailableGuns = new Dictionary<IGun, GunViewModel>();
            this.AvailableTurrets = new Dictionary<ITurret, TurretViewModel>();
            this.AvailableEngines = new Dictionary<IEngine, EngineViewModel>();
            this.AvailableRadios = new Dictionary<IRadio, RadioViewModel>();

            foreach (var chassis in this.Tank.AvailableChassis.Values)
                this.AvailableChassis[chassis] = new ChassisViewModel(this.CommandBindings, chassis, this);

            foreach (var turret in this.Tank.AvailableTurrets.Values)
                this.AvailableTurrets[turret] = new TurretViewModel(this.CommandBindings, turret, this);

            _gunViewModelCache = new Dictionary<IGun, GunViewModel>();
            foreach (var gun in this.Tank.AvailableTurrets.Values.SelectMany(t => t.AvailableGuns.Values))
                _gunViewModelCache.Add(gun, new GunViewModel(this.CommandBindings, gun, this));

            foreach (var engine in this.Tank.AvailableEngines.Values)
                this.AvailableEngines[engine] = new EngineViewModel(this.CommandBindings, engine, this);

            foreach (var radio in this.Tank.AvailableRadios.Values)
                this.AvailableRadios[radio] = new RadioViewModel(this.CommandBindings, radio, this);

            if (this.IsTdorSPG)
                foreach (var item in _gunViewModelCache)
                {
                    if (!this.AvailableGuns.Any(gun => gun.Key.Key == item.Key.Key))
                        this.AvailableGuns.Add(item.Key, item.Value);
                }

            this.LoadStockConfig();
        }

        public bool IsEliteModule(IGun gun)
        {
            var turret = this.Tank.AvailableTurrets.Values.Last();
            var maxTier = turret.AvailableGuns.Values.Max(g => g.Tier);
            return gun.Key == turret.AvailableGuns.Values.Where(g => g.Tier == maxTier).Last().Key;
        }

        public bool IsEliteModule(ITurret turret)
        {
            return turret == this.Tank.AvailableTurrets.Values.Last();
        }

        public bool IsEliteModule(IChassis chassis)
        {
            return chassis == this.Tank.AvailableChassis.Values.Last();
        }

        public bool IsEliteModule(IRadio radio)
        {
            return radio == this.Tank.AvailableRadios.Values.Last();
        }

        public bool IsEliteModule(IEngine engine)
        {
            return engine == this.Tank.AvailableEngines.Values.Last();
        }

        private IGun GetStockGun(ITurret turret)
        {
            return turret.AvailableGuns.Values.FirstOrDefault();
        }

        private IGun GetEliteGun(ITurret turret)
        {
            var maxTier = turret.AvailableGuns.Values.Max(gun => gun.Tier);
            return turret.AvailableGuns.Values.Where(gun => gun.Tier == maxTier).LastOrDefault();
        }


        protected void BeginChangeModules()
        {
            ++_changeModulesProcedureCount;
        }

        protected void EndChangeModules()
        {
            --_changeModulesProcedureCount;
            if (_changeModulesProcedureCount == 0)
                this.OnModulesChanged();
        }

        private void NotifyWeightChanged()
        {
            this.RaisePropertyChanged(() => this.Weight);
            foreach (var module in this.AllChangableModules)
                module.NotifyIsLoadCapableChanged();
        }

        protected virtual void OnModulesChanged()
        {
            this.RaisePropertyChanged(() => this.IsElite);
            this.RaisePropertyChanged(() => this.TaxologyDescription);
            this.NotifyWeightChanged();
        }

        internal virtual void LoadChassis(IChassis chassis)
        {
            this.BeginChangeModules();
            this.LoadedModules.Chassis = this.AvailableChassis[chassis];
            this.EndChangeModules();
        }

        internal virtual void LoadEngine(IEngine engine, bool validateLoadCapacity = true)
        {
            this.BeginChangeModules();
            this.LoadedModules.Engine = this.AvailableEngines[engine];
            if (validateLoadCapacity)
                this.TryUpgradeChassis();

            this.EndChangeModules();
        }

        internal virtual void LoadRadio(IRadio radio, bool validateLoadCapacity = true)
        {
            this.BeginChangeModules();
            this.LoadedModules.Radio = this.AvailableRadios[radio];
            if (validateLoadCapacity)
                this.TryUpgradeChassis();

            this.EndChangeModules();
        }


        internal virtual void LoadTurret(ITurret turret, bool validateGun = true, bool validateLoadCapacity = true)
        {
            this.BeginChangeModules();
            this.LoadedModules.Turret = this.AvailableTurrets[turret];

            if (!this.IsTdorSPG)
            {
                this.AvailableGuns.Clear();
                foreach (var gun in turret.AvailableGuns.Values)
                    this.AvailableGuns.Add(gun, _gunViewModelCache[gun]);
            }

            if (validateGun)
            {
				if (!turret.AvailableGuns.TryGetValue(this.LoadedModules.Gun.Gun.Key, out IGun gun))
					this.LoadGun(turret.AvailableGuns.Values.FirstOrDefault(), false, false);
				else
				{
					if (gun != this.LoadedModules.Gun.Gun)   // same gun, but parameters are different in this turret, so another instance
						this.LoadGun(gun, false, false);
				}
			}

            if (validateLoadCapacity)
                this.TryUpgradeChassis();

            this.EndChangeModules();
        }

        internal void LoadGun(string key, bool validateTurret = true, bool validateLoadCapacity = true)
        {
            var sameKeyGuns = _gunViewModelCache.Keys.Where(g => g.Key == key);
            var gun = sameKeyGuns.FirstOrDefault(g => this.LoadedModules.Turret.IsCompatibleWith(g));

            if (gun == null)
                gun = sameKeyGuns.FirstOrDefault();

            if (gun != null)
                this.LoadGun(gun, validateTurret, validateLoadCapacity);
        }

        internal virtual void LoadGun(IGun gun, bool validateTurret = true, bool validateLoadCapacity = true)
        {
            this.BeginChangeModules();

            this.LoadedModules.Gun = _gunViewModelCache[gun];

            if (validateTurret || this.IsTdorSPG)
                if (!this.LoadedModules.Turret.IsCompatibleWith(gun))
                {
                    foreach (var turret in this.AvailableTurrets.Values)
                        if (turret.IsCompatibleWith(gun))
                            this.LoadTurret(turret.Turret, false, false);
                }

            if (validateLoadCapacity)
                this.TryUpgradeChassis();

            this.EndChangeModules();
        }

        private void TryUpgradeChassis()
        {
            var sumWeight = this.Weight;
            if (sumWeight > this.LoadedModules.Chassis.Chassis.MaxLoad)
            {
                foreach (var chassis in this.AvailableChassis.Values)
                {
                    if (chassis.Chassis.MaxLoad >= sumWeight)
                    {
                        this.LoadChassis(chassis.Chassis);
                        break;
                    }
                }
            }
        }

        internal void LoadSimilarShell(IShell reference)
        {
            var available = this.LoadedModules.Gun.Shots;

            var isKinetic = reference.Type.IsKineticShellType();

            var sameCurrencyType = available.Where(s => s.Shell.CurrencyType == reference.CurrencyType).ToArray();

            ShellViewModel shellVm;

            if (sameCurrencyType.Length == 0)
                shellVm = available.Last();
            else if (sameCurrencyType.Length == 1)
                shellVm = sameCurrencyType[0];
            else
            {
                var sameType = sameCurrencyType.Where(s => s.Shell.Type.IsKineticShellType() == isKinetic).ToArray();
                if (sameType.Length == 0)
                    shellVm = sameCurrencyType[0];
                else
                    shellVm = sameType.Last();
            }

            this.LoadedModules.Gun.SelectedShell = shellVm;
        }

        internal void TrainCrews(int trainingLevel)
        {
            _suppressModificationContextUpdate = true;
            foreach (var crew in this.Crews)
                crew.LastSkillTrainingLevel = trainingLevel;
            _suppressModificationContextUpdate = false;

            this.UpdateModificationContext();
        }

        internal void InheritCrews(CrewViewModelCollection targetCrews)
        {
            _suppressModificationContextUpdate = true;

            var targetCrewSet = new HashSet<CrewViewModel>(targetCrews);
            foreach (var crew in this.Crews)
            {
                var targetCrew = targetCrews.FirstOrDefault(c => c.Crew.PrimaryRole == crew.Crew.PrimaryRole);
                if (targetCrew != null)
                {
                    targetCrewSet.Remove(targetCrew);
                    crew.CopyFrom(targetCrew);
                }
            }

            _suppressModificationContextUpdate = false;

            this.UpdateModificationContext();
        }

        protected virtual void LoadModules(IChassis chassis = null, IEngine engine = null, IRadio radio = null, ITurret turret = null, IGun gun = null)
        {

            this.BeginChangeModules();

            if (engine != null)
                this.LoadEngine(engine, false);

            if (radio != null)
                this.LoadRadio(radio, false);

            if (turret != null)
                this.LoadTurret(turret, gun == null, false);

            if (gun != null)
                this.LoadGun(gun, turret == null, false);

            if (chassis == null)
                this.TryUpgradeChassis();
            else
                this.LoadChassis(chassis);

            this.EndChangeModules();
        }



        internal void LoadStockConfig()
        {
            var turret = this.Tank.AvailableTurrets.Values.FirstOrDefault();
            this.LoadModules(this.Tank.AvailableChassis.Values.FirstOrDefault(),
                this.Tank.AvailableEngines.Values.FirstOrDefault(),
                this.Tank.AvailableRadios.Values.FirstOrDefault(),
                turret,
                this.GetStockGun(turret));
        }

        internal void LoadEliteConfig()
        {
            var turret = this.Tank.AvailableTurrets.Values.LastOrDefault();
            this.LoadModules(this.Tank.AvailableChassis.Values.LastOrDefault(),
                this.Tank.AvailableEngines.Values.LastOrDefault(),
                this.Tank.AvailableRadios.Values.LastOrDefault(),
                turret,
                this.GetEliteGun(turret));
        }

        internal bool IsLoadCapableIfReplacedWith(ModuleViewModel module)
        {
            return this.LoadedModules.Where(m => m.GetType() != module.GetType())
                   .Sum(m => m.Weight) + module.Weight <= this.LoadedModules.Chassis.Chassis.MaxLoad;
        }


        internal bool IsLoadCapableIfReplacedWith(ChassisViewModel module)
        {
            return this.LoadedModules.Where(m => m.GetType() != typeof(ChassisViewModel))
                   .Sum(m => m.Weight) + module.Weight <= module.Chassis.MaxLoad;
        }

        internal bool IsLoadCapableIfReplacedWith(IAccessoryViewModel equipment)
        {
            var moduleWeight = this.ModuleWeight;
            var currentWeight = this.CurrentEquipmentSlotIndex.HasValue
                ? this.LoadedEquipments[this.CurrentEquipmentSlotIndex.Value].GetWeight(moduleWeight)
                : 0;
            return this.Weight + equipment.GetWeight(moduleWeight) - currentWeight <= this.LoadedModules.Chassis.Chassis.MaxLoad;
        }

        internal void RemoveEquipment(IEquipmentViewModel equipment)
        {
            if (equipment == _emptyEquipmentViewModel)
                return;

            var index = this.LoadedEquipments.IndexOf(equipment);
            if (index >= 0)
                this.RemoveEquipment(index);
        }

        internal void RemoveEquipment(int index)
        {
            var equipment = this.LoadedEquipments[index];
            this.LoadedEquipments[index] = _emptyEquipmentViewModel;
            if (equipment != null && equipment != _emptyEquipmentViewModel)
                equipment.NotifyIsEquippedChanged();
        }

        internal void RemoveConsumable(IConsumableViewModel consumable)
        {
            if (consumable == _emptyConsumableViewModel)
                return;

            var index = this.LoadedConsumables.IndexOf(consumable);
            if (index >= 0)
                this.RemoveConsumable(index);
        }

        internal void RemoveConsumable(int index)
        {
            var consumable = this.LoadedConsumables[index];
            this.LoadedConsumables[index] = _emptyConsumableViewModel;
            if (consumable != null && consumable != _emptyConsumableViewModel)
            {
                consumable.NotifyIsEquippedChanged();
            }
        }

        internal void RemoveEquipment()
        {
            if (this.CurrentEquipmentSlotIndex.HasValue)
            {
                this.RemoveEquipment(this.CurrentEquipmentSlotIndex.Value);
                this.CurrentEquipmentSlotIndex = null;

                this.NotifyWeightChanged();
            }
        }

        internal void RemoveConsumable()
        {
            if (this.CurrentConsumableSlotIndex.HasValue)
            {
                this.RemoveConsumable(this.CurrentConsumableSlotIndex.Value);
                this.CurrentConsumableSlotIndex = null;
            }
        }

        internal void EquipEquipment(int index, IEquipmentViewModel equipment)
        {
            this.LoadedEquipments[index] = equipment;

            this.NotifyWeightChanged();
        }

        internal void EquipEquipment(IEquipmentViewModel equipment)
        {
            if (this.CurrentEquipmentSlotIndex.HasValue)
            {
                this.EquipEquipment(this.CurrentEquipmentSlotIndex.Value, equipment);
                this.CurrentEquipmentSlotIndex = null;
            }
        }

        internal void EquipConsumable(int index, IConsumableViewModel consumable)
        {
            if (consumable.IncompatibleTags != null)

                for (int i = 0; i < this.LoadedConsumables.Count; ++i)
                {
                    foreach (var tag in this.LoadedConsumables[i].Tags)
                        if (consumable.IncompatibleTags.Contains(tag))
                            this.RemoveConsumable(this.LoadedConsumables[i]);
                }

            this.LoadedConsumables[index] = consumable;
        }

        internal void EquipConsumable(IConsumableViewModel consumable)
        {
            if (this.CurrentConsumableSlotIndex.HasValue)
            {
                this.EquipConsumable(this.CurrentConsumableSlotIndex.Value, consumable);
                this.CurrentConsumableSlotIndex = null;
            }
        }


        internal void TryLoadEquipments(IList<IEquipmentViewModel> equipments)
        {
            for (int i = 0; i < equipments.Count; ++i)
            {
                var equipmentVm = equipments[i] as EquipmentViewModel;
				if (equipmentVm != null && this.AvailableEquipments.TryGetValue(equipmentVm.Equipment, out IEquipmentViewModel myEquipmentVm))
					this.EquipEquipment(i, myEquipmentVm);
				else
					this.RemoveEquipment(i);
			}
        }

        internal void TryLoadConsumables(IList<IConsumableViewModel> consumables)
        {
            for (int i = 0; i < consumables.Count; ++i)
            {
                var consumable = consumables[i] as ConsumableViewModel;
				if (consumable != null && this.AvailableConsumables.TryGetValue(consumable.Consumable, out IConsumableViewModel myConsumableVm))
					this.EquipConsumable(i, myConsumableVm);
				else
					this.RemoveConsumable(i);
			}
        }
    }
}
