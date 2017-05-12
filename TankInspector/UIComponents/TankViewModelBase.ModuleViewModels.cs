using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class TankViewModelBase
    {
        public class ModuleViewModels : NotificationObject, IEnumerable<ModuleViewModel>
        {

            private GunViewModel _gun;
            public GunViewModel Gun
            {
                get => _gun;
	            set
                {
                    _gun = value;
                    this.RaisePropertyChanged(() => this.Gun);
                }
            }

            private ChassisViewModel _chassis;
            public ChassisViewModel Chassis
            {
                get => _chassis;
	            set
                {
                    _chassis = value;
                    this.RaisePropertyChanged(() => this.Chassis);
                }
            }

            private EngineViewModel _engine;
            public EngineViewModel Engine
            {
                get => _engine;
	            set
                {
                    _engine = value;
                    this.RaisePropertyChanged(() => this.Engine);
                }
            }

            private FuelTankViewModel _fuelTank;
            public FuelTankViewModel FuelTank
            {
                get => _fuelTank;
	            set
                {
                    _fuelTank = value;
                    this.RaisePropertyChanged(() => this.FuelTank);
                }
            }


            private HullViewModel _hull;
            public HullViewModel Hull
            {
                get => _hull;
	            set
                {
                    _hull = value;
                    this.RaisePropertyChanged(() => this.Hull);
                }
            }

            private RadioViewModel _radio;
            public RadioViewModel Radio
            {
                get => _radio;
	            set
                {
                    _radio = value;
                    this.RaisePropertyChanged(() => this.Radio);
                }
            }

            private TurretViewModel _turret;
            public TurretViewModel Turret
            {
                get => _turret;
	            set
                {
                    _turret = value;
                    this.RaisePropertyChanged(() => this.Turret);
                }
            }

            public IEnumerator<ModuleViewModel> GetEnumerator()
            {
                yield return this.Chassis;
                yield return this.Engine;
                yield return this.FuelTank;
                yield return this.Gun;
                yield return this.Hull;
                yield return this.Radio;
                yield return this.Turret;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public ModuleViewModels Clone(TankViewModelBase owner, TankViewModelCloneFlags flags)
            {
                var clone = new ModuleViewModels();

                if ((flags & TankViewModelCloneFlags.FuelTank) == TankViewModelCloneFlags.FuelTank)
                    clone.FuelTank = new FuelTankViewModel(owner.CommandBindings, this.FuelTank.FuelTank, owner);
                else
                    clone.FuelTank = this.FuelTank;

                if ((flags & TankViewModelCloneFlags.Gun) == TankViewModelCloneFlags.Gun)
                {
                    clone.Gun = new GunViewModel(owner.CommandBindings, this.Gun.Gun, owner);
                    clone.Gun.SelectedShell = clone.Gun.Shots.First(s => s.Shell == this.Gun.SelectedShell.Shell);
                }
                else
                    clone.Gun = this.Gun;

                if ((flags & TankViewModelCloneFlags.Chassis) == TankViewModelCloneFlags.Chassis)
                    clone.Chassis = new ChassisViewModel(owner.CommandBindings, this.Chassis.Chassis, owner);
                else
                    clone.Chassis = this.Chassis;

                if ((flags & TankViewModelCloneFlags.Turret) == TankViewModelCloneFlags.Turret)
                    clone.Turret = new TurretViewModel(owner.CommandBindings, this.Turret.Turret, owner);
                else
                    clone.Turret = this.Turret;

                if ((flags & TankViewModelCloneFlags.Engine) == TankViewModelCloneFlags.Engine)
                    clone.Engine = new EngineViewModel(owner.CommandBindings, this.Engine.Engine, owner);
                else
                    clone.Engine = this.Engine;

                if ((flags & TankViewModelCloneFlags.Hull) == TankViewModelCloneFlags.Hull)
                    clone.Hull = new HullViewModel(owner.CommandBindings, this.Hull.Hull, owner);
                else
                    clone.Hull = this.Hull;

                if ((flags & TankViewModelCloneFlags.Radio) == TankViewModelCloneFlags.Radio)
                    clone.Radio = new RadioViewModel(owner.CommandBindings, this.Radio.Radio, owner);
                else
                    clone.Radio = this.Radio;
                return clone;
            }

        }
    }
}
