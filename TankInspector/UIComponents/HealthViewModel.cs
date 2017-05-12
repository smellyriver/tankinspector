using Smellyriver.TankInspector.DataAnalysis;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class HealthViewModel: ICustomData, IValueTipContentProvider
    {
        private static string FormatValue(double value)
        {
            return value.ToString("#,0.#");
        }

        public bool CanCompare => true;
	    public double Health { get; }
        public double RegeneratedHealth { get; }

        public HealthViewModel(double health, double regeneratedHealth)
        {
            this.Health = health;
            this.RegeneratedHealth = regeneratedHealth;
        }

        public override string ToString()
        {
            return HealthViewModel.FormatValue(this.Health);
        }

        public string ToString(bool explicitSign)
        {
	        if (explicitSign && this.Health > 0)
                return "+" + this.ToString();
	        return this.ToString();
        }

        public object Subtract(object other)
        {
            var otherHealth = other as HealthViewModel;
            if (otherHealth == null)
                return this;
	        return new HealthViewModel(this.Health - otherHealth.Health, this.RegeneratedHealth - otherHealth.RegeneratedHealth);
        }


        public double GetDeltaRatio(object delta)
        {
            var deltaHealth = delta as HealthViewModel;
            if (deltaHealth == null)
                return 0.0;

            return deltaHealth.Health / Math.Max(this.Health, 1.0);
        }


        public IEnumerable<Inline> ValueTipContent
        {
            get
            {
                var pt = App.GetLocalizedString("UnitHealthPoints");
                var inlines = new List<Inline>();

                inlines.Add(new Run(string.Format(pt, this.ToString())) { FontSize = 30 });
                inlines.Add(new Run("\n"));
                if (this.Health != this.RegeneratedHealth)
                {
                    var regenerated = App.GetLocalizedString("Regenerated");
                    var regeneratedValueString = string.Format(pt, HealthViewModel.FormatValue(this.RegeneratedHealth));
                    inlines.Add(new Run(string.Format(App.GetLocalizedString("ColonSyntax"), regenerated, regeneratedValueString)) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                }
                return inlines;
            }
        }
        public double MaxHeight => double.PositiveInfinity;
    }
}
