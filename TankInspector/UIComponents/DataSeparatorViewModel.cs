using Smellyriver.TankInspector.DataAnalysis;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class DataSeparatorViewModel : DataItemViewModelBase
    {

        public override double DesiredHeight => 10;

	    public DataSeparatorViewModel(DataSeparatorDescriptor descriptor)
            : base(descriptor)
        {

        }


        public override void CompareTo(TankViewModelBase tank, bool? useInvertedComparation = null, DeltaValueDisplayMode? deltaValueDisplayMode = null)
        {
            // do nothing
        }
    }
}
