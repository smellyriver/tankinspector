using System;
using System.Linq;
using System.Timers;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class DetailedDataViewModel
    {
        private enum ComparationTargetType
        {
            EliteConfig,
            ReplacedTank,
            Tank,
            Reference,
            ReplacedGun,
            ReplacedTurret,
            ReplacedChassis,
            ReplacedEngine,
            ReplacedRadio,
            ReplacedShell,
            ReplacedEquipment,
            RemovedEquipment,
            ReplacedConsumable,
            RemovedConsumable,
            ToggledCrewSkill,
            ToggledCrewDeadOrAlive,
            AllCrewsBiALearnt,
            AllCrewsCamouflageLearnt
        }

        private TankViewModelBase _referenceTank;

        public TankViewModelBase ReferenceTank
        {
            get => _referenceTank;
	        set
            {
                _referenceTank = value;
                if (_scheduledComparationAction == null)
                    this.CompareWithReferenceTank();
            }
        }


        private class ComparationAction
        {
            public ComparationTargetType TargetType { get; }
            public object Target { get; }
            public int Index { get; }

            public ComparationAction(object target, ComparationTargetType targetType)
            {
                this.Target = target;
                this.TargetType = targetType;
            }

            public ComparationAction(object target, ComparationTargetType targetType, int index)
                : this(target, targetType)
            {
                this.Index = index;
            }
        }

        private ComparationAction _scheduledComparationAction;
        private const double ComponentComparationDelayMilliseconds = 200.0;

        private Timer _beginComponentComparationTimer;


        private bool _isComparing;
        public bool IsComparing
        {
            get => _isComparing;
	        set
            {
                _isComparing = value;
                this.RaisePropertyChanged(() => this.IsComparing);
            }
        }

        private string _comparationTitle;
        public string ComparationTitle
        {
            get => _comparationTitle;
	        set
            {
                _comparationTitle = value;
                this.RaisePropertyChanged(() => this.ComparationTitle);
            }
        }



        private void CompareWithReferenceTank()
        {
            if (_referenceTank != null)
            {
                foreach (var dataVm in _allDataViewModels)
                {
                    dataVm.DeltaValueDisplayMode = DeltaValueDisplayMode.Icon;
                    dataVm.CompareTo(_referenceTank, false);
                }
                if (this.HighlightedComponentType == DetailedDataRelatedComponentType.HasDeltaValue)
                    this.HighlightDataItemsWithDeltaValue();
            }
            else
            {
                if (this.HighlightedComponentType == DetailedDataRelatedComponentType.HasDeltaValue)
                    this.ResetDarkenStates();
            }
        }


        private void CompareWithTank(TankViewModelBase tank, DeltaValueDisplayMode deltaValueDisplayMode = DeltaValueDisplayMode.Value, bool swap = false)
        {

            foreach (var dataVm in _allDataViewModels)
            {
                dataVm.DeltaValueDisplayMode = deltaValueDisplayMode;
                dataVm.CompareTo(tank, swap);
            }

            this.IsComparing = tank != null;

            if (this.HighlightedComponentType == DetailedDataRelatedComponentType.HasDeltaValue)
            {
                if (this.IsComparing)
                    this.HighlightDataItemsWithDeltaValue();
                else
                    this.ResetDarkenStates();
            }
        }

        public void CompareWithReplacedGun(GunViewModel gun)
        {
            var tank = this.Tank.Clone();
            tank.LoadGun(gun.Gun.Key, false);   // load gun with key, because turret change may lead to same gun in different instance
            tank.LoadSimilarShell(this.Tank.LoadedModules.Gun.SelectedShell.Shell);
            this.ComparationTitle = string.Format(App.GetLocalizedString("ComparingWithReplacedGun"), gun.Name);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        public void CompareWithReplacedTurret(TurretViewModel turret)
        {
            var tank = this.Tank.Clone();
            tank.LoadTurret(turret.Turret, true, false);
            this.ComparationTitle = string.Format(App.GetLocalizedString("ComparingWithReplacedTurret"), turret.Name);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        public void CompareWithReplacedEngine(EngineViewModel engine)
        {
            var tank = this.Tank.Clone();
            tank.LoadEngine(engine.Engine, false);
            this.ComparationTitle = string.Format(App.GetLocalizedString("ComparingWithReplacedEngine"), engine.Name);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        public void CompareWithReplacedRadio(RadioViewModel radio)
        {
            var tank = this.Tank.Clone();
            tank.LoadRadio(radio.Radio, false);
            this.ComparationTitle = string.Format(App.GetLocalizedString("ComparingWithReplacedRadio"), radio.Name);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        public void CompareWithReplacedChassis(ChassisViewModel chassis)
        {
            var tank = this.Tank.Clone();
            tank.LoadChassis(chassis.Chassis);
            this.ComparationTitle = string.Format(App.GetLocalizedString("ComparingWithReplacedChassis"), chassis.Name);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        public void CompareWithReplacedShell(ShellViewModel shell)
        {
            var tank = this.Tank.Clone();
            tank.LoadedModules.Gun.SelectedShell = shell;
            this.ComparationTitle = string.Format(App.GetLocalizedString("ComparingWithReplacedShell"), shell.Name);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }


        private void CompareWithReplacedEquipment(int index, IEquipmentViewModel equipment)
        {
            var tank = this.Tank.Clone();
            tank.EquipEquipment(index, equipment);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        private void CompareWithRemovedEquipment(int index)
        {
            var tank = this.Tank.Clone();
            tank.RemoveEquipment(index);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        private void CompareWithReplacedConsumable(int index, IConsumableViewModel consumable)
        {
            var tank = this.Tank.Clone();
            tank.EquipConsumable(index, consumable);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        private void CompareWithRemovedConsumable(int index)
        {
            var tank = this.Tank.Clone();
            tank.RemoveConsumable(index);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        private void CompareWithToggledSkill(CrewSkillViewModel skill)
        {
            var tank = this.Tank.Clone();
            tank.Crews[skill.Owner.Index].AvailableSkills.First(p => p.Key == skill.CrewSkill)
                .Value.IsLearnt = !skill.IsLearnt;
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        private void CompareWithToggledCrewDeadOrAlive(CrewViewModel crew)
        {
            var tank = this.Tank.Clone();
            tank.Crews[crew.Index].IsDead = !crew.IsDead;
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }


        private void CompareWithAllCrewsCamouflageLearnt()
        {
            var tank = this.Tank.Clone();
            tank.Crews.LearnCamouflageForAllCrewsCommand.Execute(null);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        private void CompareWithAllCrewsBiALearnt()
        {
            var tank = this.Tank.Clone();
            tank.Crews.LearnBiAForAllCrewsCommand.Execute(null);
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        private void CompareWithEliteComponent()
        {
            var tank = this.Tank.Clone();
            tank.LoadEliteConfig();
            this.CompareWithTank(tank, DeltaValueDisplayMode.Value, true);
        }

        internal void ClearComparedComponent()
        {
            if (this.IsComparing)
                this.CompareWithTank(null, DeltaValueDisplayMode.Value, false);
            this.ScheduleComparationWithConstantTank();
        }

        private void DoCompareWithReplacedComponent()
        {
            if (_scheduledComparationAction == null)
            {
                this.CompareWithReferenceTank();
                return;
            }

            var action = _scheduledComparationAction;
            _scheduledComparationAction = null;

            switch (action.TargetType)
            {
                case ComparationTargetType.EliteConfig:
                    this.CompareWithEliteComponent();
                    break;
                case ComparationTargetType.Reference:
                    this.CompareWithReferenceTank();
                    break;
                case ComparationTargetType.ReplacedChassis:
                    this.CompareWithReplacedChassis((ChassisViewModel)action.Target);
                    break;
                case ComparationTargetType.ReplacedEngine:
                    this.CompareWithReplacedEngine((EngineViewModel)action.Target);
                    break;
                case ComparationTargetType.ReplacedRadio:
                    this.CompareWithReplacedRadio((RadioViewModel)action.Target);
                    break;
                case ComparationTargetType.ReplacedShell:
                    this.CompareWithReplacedShell((ShellViewModel)action.Target);
                    break;
                case ComparationTargetType.ReplacedGun:
                    this.CompareWithReplacedGun((GunViewModel)action.Target);
                    break;
                case ComparationTargetType.ReplacedTurret:
                    this.CompareWithReplacedTurret((TurretViewModel)action.Target);
                    break;
                case ComparationTargetType.ReplacedTank:
                    this.CompareWithTank((TankViewModelBase)action.Target, DeltaValueDisplayMode.Value, true);
                    break;
                case ComparationTargetType.Tank:
                    this.CompareWithTank((TankViewModelBase)action.Target, DeltaValueDisplayMode.Value, false);
                    break;
                case ComparationTargetType.RemovedConsumable:
                    this.CompareWithRemovedConsumable(action.Index);
                    break;
                case ComparationTargetType.ReplacedConsumable:
                    this.CompareWithReplacedConsumable(action.Index, (IConsumableViewModel)action.Target);
                    break;
                case ComparationTargetType.RemovedEquipment:
                    this.CompareWithRemovedEquipment(action.Index);
                    break;
                case ComparationTargetType.ReplacedEquipment:
                    this.CompareWithReplacedEquipment(action.Index, (IEquipmentViewModel)action.Target);
                    break;
                case ComparationTargetType.ToggledCrewSkill:
                    this.CompareWithToggledSkill((CrewSkillViewModel)action.Target);
                    break;
                case ComparationTargetType.ToggledCrewDeadOrAlive:
                    this.CompareWithToggledCrewDeadOrAlive((CrewViewModel)action.Target);
                    break;
                case ComparationTargetType.AllCrewsBiALearnt:
                    this.CompareWithAllCrewsBiALearnt();
                    break;
                case ComparationTargetType.AllCrewsCamouflageLearnt:
                    this.CompareWithAllCrewsCamouflageLearnt();
                    break;

            }

        }


        internal void ScheduleComparationWithReplacedComponent(object viewModel)
        {
            ComparationTargetType targetType;
            if (viewModel is GunViewModel)
                targetType = ComparationTargetType.ReplacedGun;
            else if (viewModel is TurretViewModel)
                targetType = ComparationTargetType.ReplacedTurret;
            else if (viewModel is ChassisViewModel)
                targetType = ComparationTargetType.ReplacedChassis;
            else if (viewModel is EngineViewModel)
                targetType = ComparationTargetType.ReplacedEngine;
            else if (viewModel is RadioViewModel)
                targetType = ComparationTargetType.ReplacedRadio;
            else if (viewModel is ShellViewModel)
                targetType = ComparationTargetType.ReplacedShell;
            else
                throw new NotSupportedException();

            _scheduledComparationAction = new ComparationAction(viewModel, targetType);
            this.ResetComponentComparationTimer();
        }


        internal void ScheduleComparationWithReplacedEquipment(int index, IEquipmentViewModel equipment)
        {
            if (equipment is RemoveEquipmentViewModel)
                _scheduledComparationAction = new ComparationAction(null, ComparationTargetType.RemovedEquipment, index);
            else
                _scheduledComparationAction = new ComparationAction(equipment, ComparationTargetType.ReplacedEquipment, index);

            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithReplacedConsumable(int index, IConsumableViewModel consumable)
        {
            if (consumable is RemoveConsumableViewModel)
                _scheduledComparationAction = new ComparationAction(null, ComparationTargetType.RemovedConsumable, index);
            else
                _scheduledComparationAction = new ComparationAction(consumable, ComparationTargetType.ReplacedConsumable, index);

            this.ResetComponentComparationTimer();
        }

        private void ResetComponentComparationTimer()
        {
            if (_beginComponentComparationTimer == null)
            {
                _beginComponentComparationTimer = new Timer(ComponentComparationDelayMilliseconds);
                _beginComponentComparationTimer.Elapsed += _beginComponentComparationTimer_Elapsed;
            }

            _beginComponentComparationTimer.Stop();
            _beginComponentComparationTimer.Start();
        }

	    private void _beginComponentComparationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _dispatcher.BeginInvoke(new Action(() => this.DoCompareWithReplacedComponent()));
            _beginComponentComparationTimer.Stop();
        }


        internal void ScheduleComparationWithEliteComponent()
        {
            _scheduledComparationAction = new ComparationAction(null, ComparationTargetType.EliteConfig);
            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithReplacedTank(TankViewModelBase tank)
        {
            _scheduledComparationAction = new ComparationAction(tank, ComparationTargetType.ReplacedTank);
            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithTank(TankViewModelBase tank)
        {
            _scheduledComparationAction = new ComparationAction(tank, ComparationTargetType.Tank);
            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithConstantTank()
        {
            _scheduledComparationAction = new ComparationAction(null, ComparationTargetType.Reference);
            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithToggledCrewSkill(CrewSkillViewModel skill)
        {
            _scheduledComparationAction = new ComparationAction(skill, ComparationTargetType.ToggledCrewSkill);
            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithToggledCrewDeadOrAlive(CrewViewModel crew)
        {
            _scheduledComparationAction = new ComparationAction(crew, ComparationTargetType.ToggledCrewDeadOrAlive);
            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithAllCrewsCamouflageLearnt()
        {
            _scheduledComparationAction = new ComparationAction(null, ComparationTargetType.AllCrewsCamouflageLearnt);
            this.ResetComponentComparationTimer();
        }

        internal void ScheduleComparationWithAllCrewsBiALearnt()
        {
            _scheduledComparationAction = new ComparationAction(null, ComparationTargetType.AllCrewsBiALearnt);
            this.ResetComponentComparationTimer();
        }
    }
}
