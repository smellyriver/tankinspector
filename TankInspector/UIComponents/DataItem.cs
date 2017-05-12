using Smellyriver.TankInspector.DataAnalysis;
using System;
using System.ComponentModel;
using System.Windows;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class DataItem : NotificationObject, IDisposable
    {

        public struct DeltaInfo
        {
            public string Value { get; set; }
            public double Ratio { get; set; }
            public bool IconMode { get; set; }
            public ComparisonMode ComparationMode { get; set; }

            public override bool Equals(object obj)
            {
	            if (obj is DeltaInfo)
                {
                    var other = (DeltaInfo)obj;
                    return other.Value == this.Value && other.Ratio == this.Ratio && other.IconMode == this.IconMode;
                }
	            return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

        }

	    public DataItemDescriptorBase Descriptor { get; }

	    private TankViewModelBase _tank;

        public virtual TankViewModelBase Tank
        {
            get => _tank;
	        set
            {
                _tank = value;
                this.NotifyValueChanged();
            }
        }

        public string Unit => this.Descriptor.Unit;
	    public string Description => this.Descriptor.Description;
	    public UIElement ValueTipContentElement => this.Descriptor.ValueTipContentElement;


	    private TankViewModelBase _comparedTank;
        public virtual TankViewModelBase ComparedTank
        {
            get => _comparedTank;
	        set
            {
                var oldValue = _comparedTank;
                _comparedTank = value;

                if ((oldValue == null && value == null) || (oldValue != null && value != null))
                    this.RaisePropertyChanged("HasComparedValue");

                this.NotifyComparedValueChanged();
            }
        }

        private bool _useInvertedComparation;
        public bool UseInvertedComparation
        {
            get => _useInvertedComparation;
	        set
            {
                _useInvertedComparation = value;
                this.RaisePropertyChanged(() => this.UseInvertedComparation);

                this.NotifyDeltaValueChanged();
            }
        }
        public virtual string Value
        {
            get
            {
                if (_tank == null)
                    return null;

                return this.Descriptor.GetValue(_tank, true);
            }
        }

        public virtual bool HasComparedValue => this.Descriptor.ComparationMode != ComparisonMode.NotComparable
                                                && this.Tank != null
                                                && this.ComparedTank != null;

	    public virtual string ComparedValue => this.Descriptor.GetValue(_comparedTank, false);

	    private double _deltaRatio;
        public virtual double DeltaRatio => _deltaRatio;

	    public virtual string DeltaValue
        {
            get
            {
                if (!this.HasComparedValue)
                    return null;

                if (_useInvertedComparation)
                    return this.Descriptor.GetDeltaValue(_comparedTank, _tank, out _deltaRatio);
	            return this.Descriptor.GetDeltaValue(_tank, _comparedTank, out _deltaRatio);
            }
        }

        public DeltaInfo Delta => new DeltaInfo
        {
	        Value = this.DeltaValue,
	        Ratio = this.DeltaRatio,
	        IconMode = this.DeltaValueDisplayMode == DeltaValueDisplayMode.Icon,
	        ComparationMode = this.Descriptor.ComparationMode
        };

	    public bool IsComparedValueShown => this.DeltaValueDisplayMode == DeltaValueDisplayMode.Value;

	    private DeltaValueDisplayMode _deltaValueDisplayMode;
        public DeltaValueDisplayMode DeltaValueDisplayMode
        {
            get => _deltaValueDisplayMode;
	        set
            {
                _deltaValueDisplayMode = value;
                this.RaisePropertyChanged(() => this.DeltaValueDisplayMode);
                this.RaisePropertyChanged(() => this.IsComparedValueShown);
            }
        }



        private bool _isDarken;
        public bool IsDarken
        {
            get => _isDarken;
	        internal set
            {
                _isDarken = value;
                this.RaisePropertyChanged(() => this.IsDarken);
            }
        }

        public DataItem(DataItemDescriptorBase descriptor)
        {
            this.Descriptor = descriptor;

            this.Descriptor.PropertyChanged += _descriptor_PropertyChanged;
        }

	    private void _descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Unit" || e.PropertyName == "ValueTip")
                this.RaisePropertyChanged(e.PropertyName);
        }

        internal void NotifyValueChanged()
        {
            this.RaisePropertyChanged(() => this.Value);
            this.RaisePropertyChanged(() => this.ValueTipContentElement);
            this.NotifyDeltaValueChanged();
        }

        protected void NotifyComparedValueChanged()
        {
            this.RaisePropertyChanged("ComparedValue");
            this.NotifyDeltaValueChanged();
        }

        protected void NotifyDeltaValueChanged()
        {
            this.RaisePropertyChanged("DeltaValue");
            this.RaisePropertyChanged("DeltaRatio");
            this.RaisePropertyChanged("Delta");
        }

        public void CompareTo(TankViewModelBase tank, bool? useInvertedComparation = null, DeltaValueDisplayMode? deltaValueDisplayMode = null)
        {
            var oldValue = _comparedTank;

            if (useInvertedComparation != null && useInvertedComparation.Value != _useInvertedComparation)
            {
                _useInvertedComparation = useInvertedComparation.Value;
                this.RaisePropertyChanged("UseInvertedComparation");
            }

            if (deltaValueDisplayMode != null && deltaValueDisplayMode.Value != _deltaValueDisplayMode)
            {
                _deltaValueDisplayMode = deltaValueDisplayMode.Value;
                this.RaisePropertyChanged("DeltaValueDisplayMode");
            }

            this.ComparedTank = tank;
        }


        public virtual void Dispose()
        {

        }
    }
}
