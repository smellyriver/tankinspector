using Smellyriver.TankInspector.DataAnalysis;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CamouflageValueComponentWithCamouflageNetViewModel : CamouflageValueComponentViewModel, IValueTipContentProvider
    {
        public double? NormalValue { get; }
        public double? SecondsBeforeCamouflageNetActivate { get; }

        public CamouflageValueComponentWithCamouflageNetViewModel(double? valueWithCamouflageNet, double? normalValue, double? secondsBeforeCamouflageNetActivate)
            : base(valueWithCamouflageNet)
        {
            this.NormalValue = normalValue;
            this.SecondsBeforeCamouflageNetActivate = secondsBeforeCamouflageNetActivate;
        }


        public IEnumerable<Inline> ValueTipContent
        {
            get
            {

                var inlines = new List<Inline>();

                if (this.NormalValue != null && this.SecondsBeforeCamouflageNetActivate != null)
                {
                    return new[]
                    {
                        new Run(string.Format(App.GetLocalizedString("SecondsBeforeCamouflageNetActivate"), this.SecondsBeforeCamouflageNetActivate.Value)) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize },
                        new Run("\n"),
                        new Run(this.ToString()) { FontSize = 34 },
                        new Run("\n"),
                        new Run(string.Format(App.GetLocalizedString("NormalCamouflageValue"), CamouflageValueComponentViewModel.FormatValue(this.NormalValue.Value))) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize },
                    };
                }
	            return new[]
	            {
		            new Run(this.ToString()) { FontSize = DataItemDescriptorBase.StandardTipContentFontSize },
	            };
            }
        }



        public double MaxHeight => double.PositiveInfinity;
    }
}
