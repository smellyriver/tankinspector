using Smellyriver.TankInspector.UIComponents;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal class DataSeparatorDescriptor : DataItemViewDescriptorBase
    {
        public override int GetPriority(TankViewModelBase tank)
        {
            return int.MaxValue;
        }

    }
}
