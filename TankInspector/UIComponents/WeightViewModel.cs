using Smellyriver.TankInspector.DataAnalysis;
using System;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class WeightViewModel : ICustomData, IUnitProvider
    {
        private enum UnitMode
        {
            Auto,
            Kilograms,
            Tons,
        }

        public bool CanCompare => true;

	    private static string FormatWeight(double weight, UnitMode unitMode)
	    {
		    if (unitMode == UnitMode.Tons || (unitMode == UnitMode.Auto && weight > 1000))
                return (weight / 1000).ToString("#,0.##");
		    return weight.ToString("#,0");
	    }

        private static string GetUnitL10NKey(double weight, UnitMode unitMode)
        {
	        if (unitMode == UnitMode.Tons || (unitMode == UnitMode.Auto && weight > 1000))
                return App.GetLocalizedString("UnitTons");
	        return App.GetLocalizedString("UnitKilograms");
        }

        public double Weight { get; }

        private readonly UnitMode _unitMode;

        private UnitMode ActualUnitMode
        {
            get
            {
	            if (_unitMode == UnitMode.Auto)
                    return this.Weight > 1000 ? UnitMode.Tons : UnitMode.Kilograms;
	            return _unitMode;
            }
        }

        public WeightViewModel(double weight)
            : this(weight, UnitMode.Auto)
        {

        }

        private WeightViewModel(double weight, UnitMode unitMode)
        {
            this.Weight = weight;
            _unitMode = unitMode;
        }

        public override string ToString()
        {
            return WeightViewModel.FormatWeight(this.Weight, _unitMode);
        }

        public string ToString(bool explicitSign)
        {
	        if (explicitSign && this.Weight > 0)
                return "+" + this.ToString();
	        return this.ToString();
        }

        public string Unit => WeightViewModel.GetUnitL10NKey(this.Weight, _unitMode);

	    public object Subtract(object other)
        {
            var otherWeight = other as WeightViewModel;
            if (otherWeight == null)
                return this;

            var delta = this.Weight - otherWeight.Weight;

            return new WeightViewModel(delta, this.ActualUnitMode);
        }

        public double GetDeltaRatio(object delta)
        {
            var deltaWeight = delta as WeightViewModel;
            if (deltaWeight == null)
                return 0;

            return deltaWeight.Weight / Math.Max(this.Weight, 1.0);
        }



    }
}
