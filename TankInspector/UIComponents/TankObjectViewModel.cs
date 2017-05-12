using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class TankObjectViewModel : CommodityViewModel
    {
        public ITankObject TankObject => (ITankObject)this.Commodity;

	    public virtual string Name => this.TankObject.Name;
	    public int Tier => this.TankObject.Tier;

	    public TankObjectViewModel(CommandBindingCollection commandBindings, ITankObject tankObject)
            :base(commandBindings, tankObject)
        {

        }
    }
}
