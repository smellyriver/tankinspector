using System.Linq;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class ModelViewModel
    {

        private bool _isHitTestEnabled;
        public bool IsHitTestEnabled
        {
            get => _isHitTestEnabled;
	        set
            {
                _isHitTestEnabled = value;
                this.RaisePropertyChanged(() => this.IsHitTestEnabled);
            }
        }


        public double TankThickestArmor
        {
            get 
            {
                if (_tank == null)
                {
                    return 0.0;
                }
                return _tank.RegularArmorValues.Max(); 
            }
        }

        public double TankThinnestArmor
        {
            get 
            {
                if (_tank == null)
                {
                    return 0.0;
                }
                return _tank.RegularArmorValues.Min(); 
            }
        }

        public double TankThickestSpacingArmor
        {
            get 
            {
                if (_tank == null)
                {
                    return 0.0;
                }
                return _tank.SpacingArmorValues.Max(); 
            }
        }

        public double TankThinnestSpacingArmor
        {
            get 
            {
                if (_tank == null)
                {
                    return 0.0;
                }
                return _tank.SpacingArmorValues.Min(); 
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


        private double _regularArmorValueSelectionBegin;
        public double RegularArmorValueSelectionBegin
        {
            get => _regularArmorValueSelectionBegin;
	        set
            {
                _regularArmorValueSelectionBegin = value;
                this.RaisePropertyChanged(() => this.RegularArmorValueSelectionBegin);
            }
        }

        private double _regularArmorValueSelectionEnd;
        public double RegularArmorValueSelectionEnd
        {
            get => _regularArmorValueSelectionEnd;
	        set
            {
                _regularArmorValueSelectionEnd = value;
                this.RaisePropertyChanged(() => this.RegularArmorValueSelectionEnd);
            }
        }

        private double _spacingArmorValueSelectionBegin;
        public double SpacingArmorValueSelectionBegin
        {
            get => _spacingArmorValueSelectionBegin;
	        set
            {
                _spacingArmorValueSelectionBegin = value;
                this.RaisePropertyChanged(() => this.SpacingArmorValueSelectionBegin);
            }
        }

        private double _spacingArmorValueSelectionEnd;
        public double SpacingArmorValueSelectionEnd
        {
            get => _spacingArmorValueSelectionEnd;
	        set
            {
                _spacingArmorValueSelectionEnd = value;
                this.RaisePropertyChanged(() => this.SpacingArmorValueSelectionEnd);
            }
        }

        private double _regularArmorHintValue;
        public double RegularArmorHintValue
        {
            get => _regularArmorHintValue;
	        set
            {
                _regularArmorHintValue = value;
                this.RaisePropertyChanged(() => this.RegularArmorHintValue);
            }
        }

        private double _spacingArmorHintValue;
        public double SpacingArmorHintValue
        {
            get => _spacingArmorHintValue;
	        set
            {
                _spacingArmorHintValue = value;
                this.RaisePropertyChanged(() => this.SpacingArmorHintValue);
            }
        }

        private bool _hasRegularArmorHintValue;
        public bool HasRegularArmorHintValue
        {
            get => _hasRegularArmorHintValue;
	        set
            {
                _hasRegularArmorHintValue = value;
                this.RaisePropertyChanged(() => this.HasRegularArmorHintValue);
            }
        }

        private bool _hasSpacingArmorHintValue;
        public bool HasSpacingArmorHintValue
        {
            get => _hasSpacingArmorHintValue;
	        set
            {
                _hasSpacingArmorHintValue = value;
                this.RaisePropertyChanged(() => this.HasSpacingArmorHintValue);
            }
        }
    }
}
