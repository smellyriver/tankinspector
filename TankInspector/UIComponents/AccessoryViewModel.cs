using Smellyriver.TankInspector.Modeling;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class AccessoryViewModel : AccessoryViewModelBase, IAccessoryViewModel, IDatabaseObject
    {
        public Accessory Accessory { get; }

        public PriceViewModel Price { get; }

	    public override string Icon { get; }

	    public override int Tier => -1;

	    public override double GetWeight(double tankWeight)
        {
            return this.Accessory.GetWeight(tankWeight);
        }

        public override string Name => Accessory.Name;

	    public override string Description => Regex.Replace(Accessory.Description, @"\{\w+\}", "");

	    public override bool IsLoadCapable => this.Owner.IsLoadCapableIfReplacedWith(this);


	    public override bool IsElite => this.Price.CurrencyType == CurrencyType.Gold;

	    public override DetailedDataRelatedComponentType ComponentType => DetailedDataRelatedComponentType.HasModificationEffects;

	    IEnumerable<string> IDetailedDataRelatedComponent.ModificationDomains => this.ModificationDomains;

	    protected abstract IEnumerable<string> ModificationDomains { get; }

        public AccessoryViewModel(CommandBindingCollection commandBindings, Accessory accessory, TankViewModelBase owner)
            : base(commandBindings, owner)
        {
            this.Accessory = accessory;
            this.Price = new PriceViewModel(accessory.Price, accessory.CurrencyType);
            this.Icon = "gui" + accessory.Icon.Substring(2);
        }
        public void NotifyIsEquippedChanged()
        {
            this.RaisePropertyChanged(() => this.IsEquipped);
        }

        public Database Database => this.Accessory.Database;
    }
}
