using Smellyriver.TankInspector.DataAnalysis;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class PriceViewModel : ICustomData, IValueTipContentProvider
    {
        public int Price { get; }
        public CurrencyType CurrencyType { get; }
        public bool CanCompare => true;

	    private string Unit => this.CurrencyType == CurrencyType.Credit
		    ? App.GetLocalizedString("UnitCredits")
		    : App.GetLocalizedString("UnitGolds");

	    public PriceViewModel(int price, CurrencyType currencyType)
        {
            this.Price = price;
            this.CurrencyType = currencyType;
        }


        public object Subtract(object other)
        {
            var otherPrice = other as PriceViewModel;
            if (otherPrice == null)
                return this;

            if (otherPrice.CurrencyType != this.CurrencyType)
                return this;

            return new PriceViewModel(this.Price - otherPrice.Price, this.CurrencyType);
        }

        public double GetDeltaRatio(object delta)
        {
            var deltaPrice = delta as PriceViewModel;
            if (deltaPrice == null)
                return 0.0;

            if (deltaPrice.CurrencyType != this.CurrencyType)
                return 0.0;

            return deltaPrice.Price / Math.Max(this.Price, 1.0);
        }

        public override string ToString()
        {
            return this.Price.ToString("#,0");
        }

        public string ToString(bool explicitSign)
        {
	        if (explicitSign && this.Price > 0)
                return "+" + this.ToString();
	        return this.ToString();
        }
        public IEnumerable<Inline> ValueTipContent => new[]
        {
	        new Run(string.Format(this.Unit, this.ToString())) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize},
        };

	    public double MaxHeight => double.PositiveInfinity;
    }
}
