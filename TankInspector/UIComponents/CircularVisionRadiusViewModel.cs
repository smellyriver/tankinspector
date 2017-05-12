using Smellyriver.TankInspector.DataAnalysis;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CircularVisionRadiusViewModel : ICustomData, IValueTipContentProvider
    {

        private static string FormatValue(double value)
        {
            return value.ToString("#,0.#");
        }

        public double Value { get; }

        public double? NormalValue { get; }
        public double? SecondsBeforeStereoscopeActivate { get; }


        public CircularVisionRadiusViewModel(double value)
        {
            this.Value = value;
        }

        public CircularVisionRadiusViewModel(double value, double normalValue, double secondsBeforeStereoscopeActivate)
            : this(value)
        {
            this.NormalValue = normalValue;
            this.SecondsBeforeStereoscopeActivate = secondsBeforeStereoscopeActivate;
        }

        public object Subtract(object other)
        {
            var otherViewRange = other as CircularVisionRadiusViewModel;
            if (otherViewRange == null)
                return this;
	        return new CircularVisionRadiusViewModel(this.Value - otherViewRange.Value);
        }

        public double GetDeltaRatio(object delta)
        {
            var deltaViewRange = delta as CircularVisionRadiusViewModel;
            if (deltaViewRange == null)
                return 1.0;
	        return deltaViewRange.Value / Math.Max(this.Value, 0.0001);
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        public bool CanCompare => true;

	    public string ToString(bool explicitSign)
        {
            var result = CircularVisionRadiusViewModel.FormatValue(this.Value);
            if (explicitSign && this.Value > 0)
                result = "+" + result;

            return result;
        }


        public IEnumerable<Inline> ValueTipContent
        {
            get
            {

                var inlines = new List<Inline>();
                var meter = App.GetLocalizedString("UnitMeters");
                if (this.NormalValue != null && this.SecondsBeforeStereoscopeActivate != null)
                {
                    var valueString = string.Format(App.GetLocalizedString("NormalViewRangeValue"), CircularVisionRadiusViewModel.FormatValue(this.NormalValue.Value));
                    return new[]
                    {
                        new Run(string.Format(App.GetLocalizedString("SecondsBeforeStereoscopeActivate"), this.SecondsBeforeStereoscopeActivate.Value)) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize },
                        new Run("\n"),
                        new Run(string.Format(meter, this.ToString())) { FontSize = 26 },
                        new Run("\n"),
                        new Run(string.Format(meter, valueString)) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize },
                    };
                }
	            return new[]
	            {
		            new Run(string.Format(meter, this.ToString())) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize },
	            };
            }
        }
        public double MaxHeight => double.PositiveInfinity;
    }
}