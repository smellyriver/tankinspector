using Smellyriver.TankInspector.UIComponents;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal abstract class DataItemViewDescriptorBase : IDataViewDescriptor
    {

        internal IDataViewDescriptor Parent { get; set; }

        public DataItemViewDescriptorBase()
        {

        }

        public abstract int GetPriority(TankViewModelBase tank);
    }
}
