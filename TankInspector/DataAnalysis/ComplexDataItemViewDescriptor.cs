using Smellyriver.TankInspector.UIComponents;
using System.Linq;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal class ComplexDataItemViewDescriptor : DataItemViewDescriptorBase
    {
        public string NameL10NKey { get; }
        public DataItemViewDescriptorCollection Items { get; }


        public ComplexDataItemViewDescriptor(string nameL10NKey)
        {
            this.NameL10NKey = nameL10NKey;
            this.Items = new DataItemViewDescriptorCollection(this);
        }

        public override int GetPriority(TankViewModelBase tank)
        {
            return this.Items.Select(i => i.GetPriority(tank)).Max();
        }
    }
}
