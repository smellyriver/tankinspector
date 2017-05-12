using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Modeling;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class BenchmarkTankViewModel : TankViewModelBase
    {

        public new BenchmarkTank Tank => (BenchmarkTank)base.Tank;

	    public override string IconKey => TankIcon.VirtualTankIconKey;

	    public override string NationKey => null;

	    public override IEnumerable<TankViewModelBase> Predecessors => new TankViewModelBase[0];

	    public override IEnumerable<TankViewModelBase> Successors => new TankViewModelBase[0];

	    public override bool HasSuccessor => false;

	    public override bool HasPredecessor => false;

	    public void ApplyGoldPrice()
        {
            this.Tank.ApplyGoldPrice();
            this.UpdatePrice();
        }

        public void ApplyCreditPrice()
        {
            this.Tank.ApplyCreditPrice();
            this.UpdatePrice();
        }

        private void UpdatePrice()
        {
            this.Price = new PriceViewModel(this.Tank.Price, this.Tank.CurrencyType);
        }

        public BenchmarkTankViewModel(CommandBindingCollection commandBindings, ITank tank, HangarViewModel owner)
            : base(commandBindings, tank, owner)
        {

        }
    }
}
