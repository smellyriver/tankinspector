using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class GunVerticalTraverseComponentViewModel : ICustomData, IValueTipContentElementProvider
    {
	    private class SingleValueGunVerticalTraverseComponentViewModel : GunVerticalTraverseComponentViewModel
        {
            private readonly double _value;
            public override double Default => _value;
	        public override double Front => _value;
	        public override double Back => _value;
	        public override double ExtraBackPitchLimitsRange => 0;
	        public override double ExtraFrontPitchLimitsRange => 0;
	        public override double ExtraPitchLimitsTransition => 0;
	        public override bool HasExtraBackPitchLimits => false;
	        public override bool HasExtraFrontPitchLimits => false;

	        public SingleValueGunVerticalTraverseComponentViewModel(double value)
                : base(null, null)
            {
                _value = value;
            }

            protected override double GetValue(PitchLimits pitchLimits)
            {
                return _value;
            }

            protected override PitchLimits.Component[] GetPitchLimitComponents(PitchLimits pitchLimits)
            {
                return new[] { new PitchLimits.Component(0, _value), new PitchLimits.Component(1, _value) };
            }

            public override bool InverseFigure => false;
        }

        private readonly GunVerticalTraverse _verticalTraverse;

        public bool IsPost909Format => _verticalTraverse.DefaultPitchLimits.IsPost909Format;

	    public HorizontalTraverse HorizontalTraverse { get; }

        public virtual double Default => this.GetValue(_verticalTraverse.DefaultPitchLimits);

	    public virtual double Front => this.GetValue(_verticalTraverse.Front);

	    public virtual double Back => this.HasExtraBackPitchLimits ? this.GetValue(_verticalTraverse.ExtraPitchLimits.Back) : this.Default;

	    public double Left => this.Default;

	    public double Right => this.Default;

	    public double MaximumTraverse
        {
            get
            {
	            if (_verticalTraverse.DefaultPitchLimits.IsPost909Format)
                    return this.TraverseData.Max(c => c.Limit);
	            return MathEx.Max(this.Front, this.Right, this.Back, this.Left);
            }
        }

        public double MinimumTraverse
        {
            get
            {
	            if (_verticalTraverse.DefaultPitchLimits.IsPost909Format)
                    return this.TraverseData.Min(c => c.Limit);
	            return MathEx.Min(this.Front, this.Right, this.Back, this.Left);
            }
        }

        public PitchLimits.Component[] TraverseData => this.GetPitchLimitComponents(_verticalTraverse.DefaultPitchLimits);

	    public virtual bool HasExtraFrontPitchLimits => _verticalTraverse.HasExtraFrontPitchLimits && this.GetValue(_verticalTraverse.ExtraPitchLimits.Front) != this.GetValue(_verticalTraverse.DefaultPitchLimits);

	    public virtual bool HasExtraBackPitchLimits => _verticalTraverse.HasExtraBackPitchLimits && this.GetValue(_verticalTraverse.ExtraPitchLimits.Back) != this.GetValue(_verticalTraverse.DefaultPitchLimits);

	    public virtual double ExtraFrontPitchLimitsRange => _verticalTraverse.HasExtraFrontPitchLimits ? _verticalTraverse.ExtraPitchLimits.Front.Range : 0;

	    public virtual double ExtraBackPitchLimitsRange => _verticalTraverse.HasExtraBackPitchLimits ? _verticalTraverse.ExtraPitchLimits.Back.Range : 0;

	    public virtual double ExtraPitchLimitsTransition => _verticalTraverse.ExtraPitchLimits == null ? 0 : _verticalTraverse.ExtraPitchLimits.Transition;

	    public bool HasSingularValue
        {
            get
            {
                if (_verticalTraverse.DefaultPitchLimits.IsPost909Format)
                {
                    var data = this.TraverseData;
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    return data.Length == 1 || (data.Length == 2 && data[0].Limit == data[1].Limit);
                }

                return !this.HasExtraFrontPitchLimits && !this.HasExtraBackPitchLimits;
            }

        }

        protected GunVerticalTraverseComponentViewModel(GunVerticalTraverse vertical, HorizontalTraverse horizontal)
        {
            _verticalTraverse = vertical;
            this.HorizontalTraverse = horizontal;
        }

        protected abstract double GetValue(PitchLimits pitchLimits);
        protected abstract PitchLimits.Component[] GetPitchLimitComponents(PitchLimits pitchLimits);
        public abstract bool InverseFigure { get; }

        public object Subtract(object other)
        {
            var otherTraverse = other as GunVerticalTraverseComponentViewModel;
            if (otherTraverse == null)
                return this;

            return new SingleValueGunVerticalTraverseComponentViewModel(this.Front - otherTraverse.Front);
        }

        public double GetDeltaRatio(object delta)
        {
            var deltaTraverse = delta as GunVerticalTraverseComponentViewModel;
            if (deltaTraverse == null)
                return 0.0;

            return deltaTraverse.Front / Math.Max(this.Front, 1.0);
        }

        public bool CanCompare => true;

	    public override string ToString()
        {
            return $"{this.Front:#,0.#}";
        }

        public string ToString(bool explicitSign)
        {
	        if (explicitSign && this.Front > 0)
                return "+" + this.ToString();
	        return this.ToString();
        }

        public UIElement ValueTipContentElement
        {
            get
            {
                if (!this.HasSingularValue)
                {
                    var graph = new GunVerticalTraverseComponentView();
                    graph.VerticalTraverse = this;

                    return graph;
                }
	            var textBlock = new TextBlock();
	            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
	            textBlock.Inlines.Add(new Run(string.Format(App.GetLocalizedString("UnitDegrees"), this.ToString())) { FontSize = 32 });

	            var viewbox = new Viewbox();
	            viewbox.MaxHeight = 100;
	            viewbox.Child = textBlock;

	            return viewbox;
            }
        }
    }
}
