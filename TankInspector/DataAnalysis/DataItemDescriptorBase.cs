using Smellyriver.TankInspector.UIComponents;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Smellyriver.Utilities;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal abstract class DataItemDescriptorBase : NotificationObject
    {
        public const double StandardTipContentFontSize = 12.0;
        public const double StandardTipContentUnitFontSize = 10.0;

        public abstract string FullNameL10NKey { get; }
        public abstract string Unit { get; }
        public abstract string Description { get; }
        public UIElement ValueTipContentElement { get; private set; }
        public abstract string Format { get; }
        public abstract bool IsDecimal { get; }
        public abstract double BenchmarkRatio { get; }
        public abstract ComparisonMode ComparationMode { get; }

        protected abstract object GetRawValue(TankViewModelBase tank, bool updateRelatedFields = false);

        public string GetValue(TankViewModelBase tank, bool updateRelatedFields = false)
        {
            var value = this.GetRawValue(tank, updateRelatedFields);
            var result = this.FormatValue(value, false);

            if (updateRelatedFields)
            {
                if (value is IValueTipContentElementProvider)
                    this.ValueTipContentElement = ((IValueTipContentElementProvider)value).ValueTipContentElement;
                else
                {
                    IEnumerable<Inline> valueTipContent;
                    double maxHeight;

                    if (value is IValueTipContentProvider)
                    {
                        var provider = (IValueTipContentProvider)value;
                        valueTipContent = provider.ValueTipContent;
                        maxHeight = provider.MaxHeight;
                    }
                    else
                    {
                        var inlines = new List<Inline>();

                        if (string.IsNullOrEmpty(this.Unit))
                            inlines.Add(new Run(result) { FontSize = StandardTipContentFontSize });
                        else
                            inlines.Add(new Run(string.Format(this.Unit, result)) { FontSize = StandardTipContentFontSize });

                        valueTipContent = inlines;
                        maxHeight = 100;
                    }

                    var viewbox = new Viewbox();
                    viewbox.VerticalAlignment = VerticalAlignment.Top;
                    viewbox.MaxHeight = maxHeight;
                    viewbox.Stretch = Stretch.Uniform;

                    var textblock = new TextBlock();
                    textblock.HorizontalAlignment = HorizontalAlignment.Center;
                    textblock.VerticalAlignment = VerticalAlignment.Center;
                    textblock.TextAlignment = TextAlignment.Center;
                    textblock.Inlines.AddRange(valueTipContent);
                    viewbox.Child = textblock;

                    this.ValueTipContentElement = viewbox;
                }
                this.RaisePropertyChanged(() => this.ValueTipContentElement);

            }

            return result;
        }


        public string GetDeltaValue(TankViewModelBase tank, TankViewModelBase comparedTo, out double deltaRatio)
        {
            if (this.ComparationMode == ComparisonMode.NotComparable)
            {
                deltaRatio = 0;
                return null;
            }

            var myValue = this.GetRawValue(tank, false);
            var otherValue = this.GetRawValue(comparedTo, false);

            object delta;

            var myValueComparable = myValue as ICustomData;
            if (myValueComparable != null)
            {
                if (!myValueComparable.CanCompare)
                {
                    deltaRatio = 0;
                    return null;
                }

                var otherComparable = (ICustomData)otherValue;
                delta = myValueComparable.Subtract(otherComparable);
                deltaRatio = myValueComparable.GetDeltaRatio(delta);
            }
            else
            {

                var myDoubleValue = Convert.ToDouble(myValue);
                var otherDoubleValue = Convert.ToDouble(otherValue);

                var doubleDelta = myDoubleValue - otherDoubleValue;
                delta = doubleDelta;

                if (myDoubleValue == 0)
                    deltaRatio = Math.Sign(doubleDelta);
                else
                    deltaRatio = doubleDelta / myDoubleValue * Math.Sign(myDoubleValue);
            }

            if (this.ComparationMode == ComparisonMode.LowerBetter)
                deltaRatio = -deltaRatio;

            var benchmarkRatio = Math.Abs(this.BenchmarkRatio);
            if (benchmarkRatio > double.Epsilon)
                deltaRatio = (deltaRatio / benchmarkRatio).Clamp(-1.0, 1.0);

            if (Math.Abs(deltaRatio) < 0.01)
            {
                deltaRatio = 0;
                return null;
            }
	        return this.FormatValue(delta, true);
        }

        private string FormatValue(object value, bool explicitSign)
        {
            if (value is ICustomData)
                return ((ICustomData)value).ToString(explicitSign);

            string result;
            var array = value as object[];
            if (array != null)
                result = string.Format(this.Format, array);
            else
                result = string.Format(this.Format, value);

            if (explicitSign && this.IsDecimal)
            {
                var valueDelta = double.Parse(value.ToString(), CultureInfo.InvariantCulture);
                if (valueDelta > 0 && !result.StartsWith("+"))
                    result = "+" + result;
            }

            return result;
        }

    }
}
