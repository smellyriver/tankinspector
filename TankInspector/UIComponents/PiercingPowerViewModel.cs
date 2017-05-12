using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class PiercingPowerViewModel : ICustomData, IValueTipContentProvider
    {
        private static string FormatValue(double value)
        {
            return value.ToString("#,0.#");
        }
        public bool CanCompare => true;
	    public double P100 { get; }
        public double P400 { get; }

        public double MaxPenetration => this.P100 * 1.25;
	    public double MinPenetration => this.P400 * 0.75;

	    public PiercingPowerViewModel(PiercingPower piercingPower)
            : this(piercingPower.P100, piercingPower.P400)
        {

        }

        private PiercingPowerViewModel(double p100, double p400)
        {
            this.P100 = p100;
            this.P400 = p400;
        }

        public override string ToString()
        {
            return PiercingPowerViewModel.FormatValue(this.P100);
        }

        public string ToString(bool explicitSign)
        {
	        if (explicitSign && this.P100 > 0)
                return "+" + this.ToString();
	        return this.ToString();
        }

        public object Subtract(object other)
        {
            var otherPiecingPower = other as PiercingPowerViewModel;
            if (otherPiecingPower == null)
                return this;
	        return new PiercingPowerViewModel(this.P100 - otherPiecingPower.P100, this.P400 - otherPiecingPower.P400);
        }


        public double GetDeltaRatio(object delta)
        {
            var deltaPiecingPower = delta as PiercingPowerViewModel;
            if (deltaPiecingPower == null)
                return 0.0;

            return deltaPiecingPower.P100 / Math.Max(this.P100, 1.0);
        }


        public IEnumerable<Inline> ValueTipContent
        {
            get
            {
                var mm = App.GetLocalizedString("UnitMillimeters");
                var inlines = new List<Inline>();

                inlines.Add(new Run(string.Format(mm, PiercingPowerViewModel.FormatValue(this.P100))) { FontSize = 30 });
                inlines.Add(new Run("\n"));
                if (this.P100 != this.P400)
                {
                    var at400M = App.GetLocalizedString("At400m");
                    var at400MValueString = string.Format(mm, PiercingPowerViewModel.FormatValue(this.P400));
                    inlines.Add(new Run(string.Format(App.GetLocalizedString("ColonSyntax"), at400M, at400MValueString)) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                    inlines.Add(new Run("\n"));
                }
                inlines.Add(new Run(string.Format(mm, PiercingPowerViewModel.FormatValue(this.MinPenetration))) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                inlines.Add(new Run(" ~ ") { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                inlines.Add(new Run(string.Format(mm, PiercingPowerViewModel.FormatValue(this.MaxPenetration))) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                return inlines;
            }
        }

        public double MaxHeight => double.PositiveInfinity;
    }
}
