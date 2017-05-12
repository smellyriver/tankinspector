using Smellyriver.TankInspector.Graphics.Scene;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf;
using Smellyriver.Wpf.Input;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class EquivalentArmorViewModel : DependencyNotificationObject
    {


        private static bool IsSimilarShellType(ShellType type, ShellType reference)
        {
            switch (reference)
            {
                case ShellType.Ap:
                    return type == ShellType.Ap || type == ShellType.Aphe;
                case ShellType.He:
                    return type == ShellType.He || type == ShellType.PremiumHe;
                default:
                    return type == reference;
            }
        }

        private bool _isSelectShellViewShown;
        public bool IsSelectShellViewShown
        {
            get => _isSelectShellViewShown;
	        set
            {
                _isSelectShellViewShown = value;
                this.RaisePropertyChanged(() => this.IsSelectShellViewShown);
            }
        }

        private ShellType _selectedShellType;
        public ShellType SelectedShellType
        {
            get => _selectedShellType;
	        private set
            {
                _selectedShellType = value;

                ApplicationSettings.Default.TestShellType = _selectedShellType;
                ApplicationSettings.Default.Save();

                this.RaisePropertyChanged(() => this.SelectedShellType);
                this.OnShellChanged();
            }
        }


        private bool _isApShellSelected;
        public bool IsApShellSelected
        {
            get => _isApShellSelected;
	        set
            {
                _isApShellSelected = value;
                this.RaisePropertyChanged(() => this.IsApShellSelected);
                if (value)
                {
                    this.SelectedShellType = ShellType.Ap;
                    this.UpdateCalibers();
                    this.RaisePropertyChanged(() => this.SelectedShellName);
                }
            }
        }

        private bool _isApcrShellSelected;
        public bool IsApcrShellSelected
        {
            get => _isApcrShellSelected;
	        set
            {
                _isApcrShellSelected = value;
                this.RaisePropertyChanged(() => this.IsApcrShellSelected);
                if (value)
                {
                    this.SelectedShellType = ShellType.Apcr;
                    this.UpdateCalibers();
                    this.RaisePropertyChanged(() => this.SelectedShellName);
                }
            }
        }

        private bool _isHeShellSelected;
        public bool IsHeShellSelected
        {
            get => _isHeShellSelected;
	        set
            {
                _isHeShellSelected = value;
                this.RaisePropertyChanged(() => this.IsHeShellSelected);
                if (value)
                {
                    this.SelectedShellType = ShellType.He;
                    this.UpdateCalibers();
                    this.RaisePropertyChanged(() => this.SelectedShellName);
                }
            }
        }

        private bool _isHeatShellSelected;
        public bool IsHeatShellSelected
        {
            get => _isHeatShellSelected;
	        set
            {
                _isHeatShellSelected = value;
                this.RaisePropertyChanged(() => this.IsHeatShellSelected);
                if (value)
                {
                    this.SelectedShellType = ShellType.Heat;
                    this.UpdateCalibers();
                    this.RaisePropertyChanged(() => this.SelectedShellName);
                }
            }
        }

        private DoubleCollection _caliberTicks;
        public DoubleCollection CaliberTicks
        {
            get => _caliberTicks;
	        private set
            {
                _caliberTicks = value;
                this.RaisePropertyChanged(() => this.CaliberTicks);
            }
        }

        private double _minCaliber;
        public double MinCaliber
        {
            get => _minCaliber;
	        private set
            {
                _minCaliber = value;
                this.RaisePropertyChanged(() => this.MinCaliber);
            }
        }

        private double _maxCaliber;
        public double MaxCaliber
        {
            get => _maxCaliber;
	        private set
            {
                _maxCaliber = value;
                this.RaisePropertyChanged(() => this.MaxCaliber);
            }
        }

        private double _caliber;
        public double Caliber
        {
            get => _caliber;
	        set
            {
                _caliber = value;

                ApplicationSettings.Default.TestShellCaliber = _caliber;
                ApplicationSettings.Default.Save();

                this.RaisePropertyChanged(() => this.Caliber);
                this.RaisePropertyChanged(() => this.SelectedShellName);
                this.OnShellChanged();
            }
        }

        public string SelectedShellName => string.Format(App.GetLocalizedString("ShellTypeDisplay"), this.Caliber, App.GetLocalizedString(this.SelectedShellType.ToString()));

	    private bool _isUpdatingShootResult;

        public string EquivalentThicknessDisplay
        {
            get
            {
                switch (this.PenetrationState)
                {
                    case PenetrationState.NotApplicable:
                        return App.GetLocalizedString("EquivalentThicknessNotApplicable");
                    case PenetrationState.Penetratable:
                        return this.EquivalentThickness.ToString("F0");
                    case PenetrationState.Richochet:
                        return App.GetLocalizedString("EquivalentThicknessRicochet");
                    case PenetrationState.Unpenetratable:
                        return App.GetLocalizedString("EquivalentThicknessUnpenetratable");
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private double _equivalentThickness;
        public double EquivalentThickness
        {
            get { return _equivalentThickness; }
            set
            {
                if (_equivalentThickness != value)
                {
                    _equivalentThickness = value;

                    this.RaisePropertyChanged(() => this.EquivalentThickness);
                    if (!_isUpdatingShootResult)
                        this.RaisePropertyChanged(() => this.EquivalentThicknessDisplay);
                }
            }
        }

        private PenetrationState _penetrationState;
        public PenetrationState PenetrationState
        {
            get { return _penetrationState; }
            set
            {
                if (_penetrationState != value)
                {
                    _penetrationState = value;

                    this.RaisePropertyChanged(() => this.PenetrationState);
                    if (!_isUpdatingShootResult)
                        this.RaisePropertyChanged(() => this.EquivalentThicknessDisplay);
                }
            }
        }

        private double _normalizationAngle;
        public double NormalizationAngle
        {
            get { return _normalizationAngle; }
            set
            {
                if (_normalizationAngle != value)
                {
                    _normalizationAngle = value;
                    this.RaisePropertyChanged(() => this.NormalizationAngle);
                }
            }
        }

        private double _impactAngle;
        public double ImpactAngle
        {
            get { return _impactAngle; }
            set
            {
                if (_impactAngle != value)
                {
                    _impactAngle = value;
                    this.RaisePropertyChanged(() => this.ImpactAngle);
                }
            }
        }

        private bool _is2XRuleActive;
        public bool Is2XRuleActive
        {
            get { return _is2XRuleActive; }
            set
            {
                if (_is2XRuleActive != value)
                {
                    _is2XRuleActive = value;
                    this.RaisePropertyChanged(() => this.Is2XRuleActive);
                }
            }
        }

        private bool _is3XRuleActive;
        public bool Is3XRuleActive
        {
            get { return _is3XRuleActive; }
            set
            {
                if (_is3XRuleActive != value)
                {
                    _is3XRuleActive = value;
                    this.RaisePropertyChanged(() => this.Is3XRuleActive);
                }
            }
        }


        public ICommand ToggleSelectShellViewCommand { get; }

        public event EventHandler ShellChanged;
        public EquivalentArmorViewModel(CommandBindingCollection commandBindings)
        {
            this.ToggleSelectShellViewCommand = Command.FromAction(commandBindings, this.ToggleSelectShellView);
            this.LoadSettings();
            Database.CurrentDatabaseChanged += Database_CurrentDatabaseChanged;
        }

        private void UpdateCalibers()
        {
            var calibers = Database.Current.Nations.Values.SelectMany(n => n.Shells.Values)
                    .Where(s => EquivalentArmorViewModel.IsSimilarShellType(s.Type, this.SelectedShellType))
                    .Select(s => s.Caliber)
                    .Distinct()
                    .OrderBy(c => c);

            this.CaliberTicks = new DoubleCollection(calibers);

            this.MinCaliber = calibers.First();
            this.MaxCaliber = calibers.Last();

            if (this.Caliber > this.MaxCaliber)
                this.Caliber = this.MaxCaliber;
            else
                this.Caliber = calibers.First(c => c >= this.Caliber);
        }

        private void OnShellChanged()
        {
            if (this.ShellChanged != null)
                this.ShellChanged(this, new EventArgs());
        }

	    private void Database_CurrentDatabaseChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => this.LoadSettings()));
        }

        private void ToggleSelectShellView()
        {
            this.IsSelectShellViewShown = !this.IsSelectShellViewShown;
        }

        private void LoadSettings()
        {
            this.Caliber = ApplicationSettings.Default.TestShellCaliber;

            switch (ApplicationSettings.Default.TestShellType)
            {
                case ShellType.Apcr:
                    this.IsApcrShellSelected = true;
                    break;
                case ShellType.Heat:
                    this.IsHeatShellSelected = true;
                    break;
                case ShellType.He:
                case ShellType.PremiumHe:
                    this.IsHeShellSelected = true;
                    break;
                case ShellType.Ap:
                case ShellType.Aphe:
                default:
                    this.IsApShellSelected = true;
                    break;
            }
        }


        internal void BeginUpdateShootResult()
        {
            _isUpdatingShootResult = true;
        }

        internal void EndUpdateShootResult()
        {
            _isUpdatingShootResult = false;
            this.RaisePropertyChanged(() => this.EquivalentThicknessDisplay);
        }
    }
}
