using Smellyriver.TankInspector.DataAnalysis;
using System;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CamouflageValueComponentViewModel : ICustomData
    {

        public static readonly CamouflageValueComponentViewModel NotApplicable = new CamouflageValueComponentViewModel(null);
        protected static string FormatValue(double value)
        {
            return value.ToString("#,0.##%");
        }

        public double? Value { get; }

        public CamouflageValueComponentViewModel(double? value)
        {
            this.Value = value;
        }

        public virtual object Subtract(object other)
        {
            var otherCamo = other as CamouflageValueComponentViewModel;
            if (otherCamo == null || otherCamo.Value == null)
                return this;
	        return new CamouflageValueComponentViewModel(this.Value.Value - otherCamo.Value.Value);
        }

        public double GetDeltaRatio(object delta)
        {
            var deltaCamo = delta as CamouflageValueComponentViewModel;
            if (deltaCamo == null || deltaCamo.Value == null)
                return 1.0;
	        return deltaCamo.Value.Value / Math.Max(this.Value.Value, 0.0001);
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        public bool CanCompare => this.Value != null;

	    public virtual string ToString(bool explicitSign)
        {
            if (this.Value == null)
                return App.GetLocalizedString("NotApplicable");
	        var result = CamouflageValueComponentViewModel.FormatValue(this.Value.Value);
	        if (explicitSign && this.Value.Value > 0)
		        result = "+" + result;

	        return result;
        }



    }
}
