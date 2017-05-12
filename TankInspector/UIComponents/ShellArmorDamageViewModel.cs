using Smellyriver.TankInspector.DataAnalysis;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class ShellArmorDamageViewModel : ICustomData
    {
        public double ArmorDamage { get; }

        public double MaxArmorDamage => this.ArmorDamage * 1.25;
	    public double MinArmorDamage => this.ArmorDamage * 0.75;
	    public bool CanCompare => true;

	    public ShellArmorDamageViewModel(double armorDamage)
        {
            this.ArmorDamage = armorDamage;
        }

        public override string ToString()
        {
            return this.ArmorDamage.ToString("0.#");
        }

        public string ToString(bool explicitSign)
        {
	        if (explicitSign && this.ArmorDamage > 0)
                return "+" + this.ToString();
	        return this.ToString();
        }

        public object Subtract(object other)
        {
            var otherDamage = other as ShellArmorDamageViewModel;
            if (otherDamage == null)
                return this;
	        return new ShellArmorDamageViewModel(this.ArmorDamage - otherDamage.ArmorDamage);
        }

        public double GetDeltaRatio(object delta)
        {
            var deltaDamage = delta as ShellArmorDamageViewModel;
            if (deltaDamage == null)
                return 0.0;

            return deltaDamage.ArmorDamage / Math.Max(this.ArmorDamage, 1.0);
        }



        public IEnumerable<Inline> ValueTipContent
        {
            get
            {
                var pts = App.GetLocalizedString("UnitHealthPoints");
                var inlines = new List<Inline>();

                inlines.Add(new Run(string.Format(pts, this.ArmorDamage.ToString("0.#"))) { FontSize = 30 });
                inlines.Add(new Run("\n"));
                inlines.Add(new Run(string.Format(pts, this.MinArmorDamage.ToString("F0"))) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                inlines.Add(new Run(" ~ ") { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                inlines.Add(new Run(string.Format(pts,this.MaxArmorDamage.ToString("F0"))) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize });
                return inlines;
            }
        }

    }
}
