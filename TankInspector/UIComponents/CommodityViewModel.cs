using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class CommodityViewModel : DependencyNotificationObject
    {
        public ICommodity Commodity { get; }

        public PriceViewModel Price { get; }

        protected CommandBindingCollection CommandBindings { get; }

        public CommodityViewModel(CommandBindingCollection commandBindings, ICommodity commidity)
        {
            this.CommandBindings = commandBindings;
            this.Commodity = commidity;
            this.Price = new PriceViewModel(this.Commodity.Price, this.Commodity.CurrencyType);
        }
    }
}
