using log4net;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class HangarViewModel : NotificationObject, IReferenceTankProvider
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        public static readonly GradientStopCollection DefaultRegularArmorSpectrumStops;
        public static readonly GradientStopCollection DefaultSpacingArmorSpectrumStops;

        static HangarViewModel()
        {
            DefaultRegularArmorSpectrumStops = new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0, 0) , 0.0),
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0x30, 0), 0.33),
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0xc0, 0), 0.67),
                new GradientStop(Color.FromArgb(0xff, 0, 0xc0, 0), 1.0),
            };

            DefaultRegularArmorSpectrumStops.Freeze();

            DefaultSpacingArmorSpectrumStops = new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0, 0xc0), 0.0),
                new GradientStop(Color.FromArgb(0xff, 0x30, 0, 0xc0), 0.33),
                new GradientStop(Color.FromArgb(0xff, 0, 0, 0xc0), 0.67),
                new GradientStop(Color.FromArgb(0xff, 0, 0xc0, 0xc0), 1.0),
            };

            DefaultSpacingArmorSpectrumStops.Freeze();
        }

        public event EventHandler<ReferenceTankChangedEventArgs> ReferenceTankChanged;

        private TankViewModel _tank;
        public TankViewModel Tank
        {
            get => _tank;
	        private set
            {
                if (value == null)
                    Log.Info("setting selected tank: null");
                else
                    Log.InfoFormat("setting selected tank: {0}", value.Tank.ColonFullKey);

                if (_tank != null)
                {
                    _tank.LoadedModules.PropertyChanged -= this.CurrentTankLoadedModules_PropertyChanged;
                    _tank.LoadedConsumables.CollectionChanged -= CurrentTankLoadedConsumables_CollectionChanged;
                    _tank.LoadedEquipments.CollectionChanged -= CurrentTankLoadedEquipments_CollectionChanged;
                }

                _tank = value;

                if (_tank == null)
                    this.TankCurrentGun = null;
                else
                {
                    _tank.LoadedModules.PropertyChanged += this.CurrentTankLoadedModules_PropertyChanged;
                    this.Tank.LoadedConsumables.CollectionChanged += CurrentTankLoadedConsumables_CollectionChanged;
                    this.Tank.LoadedEquipments.CollectionChanged += CurrentTankLoadedEquipments_CollectionChanged;
                    this.TankCurrentGun = _tank.LoadedModules.Gun;
                }

                this.OnTankChanged();

                this.TryAssignBenchmarkTank();
            }
        }


        private GunViewModel _tankCurrentGun;
        private GunViewModel TankCurrentGun
        {
            get => _tankCurrentGun;
	        set
            {
                if (_tankCurrentGun != null)
                    _tankCurrentGun.PropertyChanged -= this.CurrentTankCurrentGun_PropertyChanged;
                _tankCurrentGun = value; ;
                if (_tankCurrentGun != null)
                    _tankCurrentGun.PropertyChanged += this.CurrentTankCurrentGun_PropertyChanged;
            }
        }

        private TankViewModelBase _referenceTank;
        public TankViewModelBase ReferenceTank
        {
            get => _referenceTank;
	        private set
            {

                if (value == null)
                    Log.Info("setting reference tank: null");
                else
                    Log.InfoFormat("setting reference tank: {0}", value.Tank.ColonFullKey);

                _referenceTank = value;

                if (_referenceTank == null || _referenceTank.Tank is BenchmarkTank)
                    this.ReferenceTankType = ReferenceTankType.Benchmark;
                else
                    this.ReferenceTankType = ReferenceTankType.Regular;
				
                this.OnReferenceTankChanged();
                
            }
        }


        private GradientStopCollection _regularArmorSpectrumStops;
        public GradientStopCollection RegularArmorSpectrumStops
        {
            get => _regularArmorSpectrumStops;
	        set
            {
                _regularArmorSpectrumStops = value;
                this.RaisePropertyChanged(() => this.RegularArmorSpectrumStops);
            }
        }

        private GradientStopCollection _spacingArmorSpectrumStops;
        public GradientStopCollection SpacingArmorSpectrumStops
        {
            get => _spacingArmorSpectrumStops;
	        set
            {
                _spacingArmorSpectrumStops = value;
                this.RaisePropertyChanged(() => this.SpacingArmorSpectrumStops);
            }
        }

        private bool _isTakingPhoto;
        public bool IsTakingPhoto
        {
            get => _isTakingPhoto;
	        set
            {
                _isTakingPhoto = value;
                this.RaisePropertyChanged(() => this.IsTakingPhoto);
            }
        }


        TankViewModelBase IReferenceTankProvider.ReferenceTank => this.ReferenceTank;

	    public ReferenceTankType ReferenceTankType { get; private set; }
        public ModelViewModel ModelView { get; }
        public DetailedDataViewModel DetailedDataView { get; }

        public EquivalentArmorViewModel EquivalentArmorView { get; }

        private readonly MainWindowViewModel _owner;

        private readonly CommandBindingCollection _commandBindings;
        public ICommand LoadEliteConfigCommand { get; private set; }
        public ICommand TurnToLeftPageCommand { get; private set; }
        public ICommand TurnToRightPageCommand { get; private set; }
        public ICommand SearchForReferenceTanksCommand { get; private set; }
        public ICommand ToggleTankDescriptionVisibilityCommand { get; private set; }

        public ICommand SwitchReferenceTankCommand { get; private set; }
        public HangarViewModel(CommandBindingCollection commandBindings, MainWindowViewModel owner)
        {
            _commandBindings = commandBindings;
            _owner = owner;

            this.RegularArmorSpectrumStops = DefaultRegularArmorSpectrumStops;
            this.SpacingArmorSpectrumStops = DefaultSpacingArmorSpectrumStops;

            this.InitializeCommands();

            this.IsSidebarShown = false;

            this.ModelView = new ModelViewModel(commandBindings) { ModelType = Graphics.Model.ModelType.Undamaged };
            this.ModelView.RegularArmorSpectrumStops = DefaultRegularArmorSpectrumStops;
            this.ModelView.SpacingArmorSpectrumStops = DefaultSpacingArmorSpectrumStops;

            this.DetailedDataView = new DetailedDataViewModel(this.Tank, this);

            this.EquivalentArmorView = new EquivalentArmorViewModel(commandBindings);

            this.IsModelViewShown = true;
        }
        
        private void InitializeCommands()
        {
            this.LoadEliteConfigCommand = new RelayCommand(() => this.Tank.LoadEliteConfig());
            this.TurnToLeftPageCommand = new RelayCommand(this.TurnToLeftPage);
            this.TurnToRightPageCommand = new RelayCommand(this.TurnToRightPage);
            this.SearchForReferenceTanksCommand = new RelayCommand(this.SearchForReferenceTanks);
            this.ToggleTankDescriptionVisibilityCommand = new RelayCommand(this.ToggleTankDescriptionVisibility);
            this.SwitchReferenceTankCommand = new RelayCommand(this.SwitchReferenceTank);
        }

        private void SwitchReferenceTank()
        {
            if (!this.CanSwitchReferenceTank)
                return;

            var tank = _tank;
            _tank = (TankViewModel)this.ReferenceTank;
            _referenceTank = tank;

            this.OnTankChanged();
            this.OnReferenceTankChanged();
        }

        public void LoadTank(Tank tank)
        {
            // do not assign to this.Tank directly, otherwise the event raised by the setter
            // will lead to race problem
            var tankVm = new TankViewModel(_commandBindings, tank, this);

            this.ConfigureTank(tankVm);
            this.InheritCurrentTank(tankVm);

            this.Tank = tankVm;
        }


        private void OnTankChanged()
        {
            this.ModelView.Tank = _tank;
            this.DetailedDataView.Tank = _tank;

            ApplicationSettings.Default.PreviousTankKey = _tank.Tank.ColonFullKey;
            ApplicationSettings.Default.Save();

            this.RaisePropertyChanged(() => this.Tank);
            this.RaisePropertyChanged(() => this.IsPredecessorPanelShown);
            this.RaisePropertyChanged(() => this.IsSuccessorPanelShown);
        }


        private void OnReferenceTankChanged()
        {
            this.RaisePropertyChanged(() => this.ReferenceTank);
            this.RaisePropertyChanged(() => this.CanSwitchReferenceTank);

            if (this.ReferenceTankChanged != null)
                this.ReferenceTankChanged(this, new ReferenceTankChangedEventArgs(_referenceTank));
        }

        public void ConfigureTank(TankViewModelBase tank)
        {
            if (ApplicationSettings.Default.AutoLoadEliteConfigEnabled)
                tank.LoadEliteConfig();

            if (ApplicationSettings.Default.RecruitFullyTrainedCrews)
                tank.TrainCrews(100);
        }

        public void InheritCurrentTank(TankViewModelBase tank)
        {
            if (this.Tank != null)
            {
                if (!ApplicationSettings.Default.ClearEquipmentsOnChangingVehicle)
                    tank.TryLoadEquipments(this.Tank.LoadedEquipments);

                if (!ApplicationSettings.Default.ClearConsumablesOnChangingVehicle)
                    tank.TryLoadConsumables(this.Tank.LoadedConsumables);

                if (!ApplicationSettings.Default.RecruitNewCrewsOnChangingVehicle)
                    tank.InheritCrews(this.Tank.Crews);

                if (!ApplicationSettings.Default.ResetShellTypeOnChangingVehicle)
                    tank.LoadSimilarShell(this.Tank.LoadedModules.Gun.SelectedShell.Shell);
            }
        }


	    private void CurrentTankCurrentGun_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedShell")
            {
                if (this.ReferenceTank == this.Tank)
                    return;

                this.ReferenceTank.LoadSimilarShell(this.TankCurrentGun.SelectedShell.Shell);
            }
        }

	    private void CurrentTankLoadedModules_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Gun")
            {
                this.TankCurrentGun = this.Tank.LoadedModules.Gun;
            }
        }


	    private void CurrentTankLoadedEquipments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (this.ReferenceTank != null)
            //    this.ReferenceTank.TryLoadEquipments(this.Tank.LoadedEquipments);
        }

	    private void CurrentTankLoadedConsumables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (this.ReferenceTank != null)
            //    this.ReferenceTank.TryLoadConsumables(this.Tank.LoadedConsumables);
        }

        private void TryAssignBenchmarkTank()
        {
            if (this.ReferenceTankType == ReferenceTankType.Benchmark || ApplicationSettings.Default.AutoResetReferenceTankEnabled)
            {
                var tank = this.Tank.Database.GetBenchmarkTank(this.Tank.Tier, this.Tank.Tank.Class);

                if (tank == null)
                    this.ReferenceTank = this.Tank;
                else
                {
                    var tankVm = new BenchmarkTankViewModel(_commandBindings, tank, this);

                    this.SetReferenceTankDefaultState(tankVm);

                    if (this.Tank.Tank.IsPremiumTank)
                        tankVm.ApplyGoldPrice();
                    else
                        tankVm.ApplyCreditPrice();


                    this.ReferenceTank = tankVm;
                }
            }
        }

        private void SetReferenceTankDefaultState(TankViewModelBase reference)
        {
            reference.LoadEliteConfig();
            reference.TrainCrews(100);
            reference.LoadSimilarShell(this.Tank.LoadedModules.Gun.SelectedShell.Shell);
        }
    }
}
