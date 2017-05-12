using Smellyriver.TankInspector.DataAnalysis;
using System.Diagnostics;

namespace Smellyriver.TankInspector.UIComponents
{
    [DebuggerDisplay("{FullName}")]
    internal class DataItemViewModel : DataItemViewModelBase
    {

        public string Name { get; }

        public string FullName { get; }

        public DataItem DataItem { get; }

        public bool IsSubItem { get; set; }

        public override TankViewModelBase Tank
        {
            get => base.Tank;
	        set
            {
                base.Tank = value;
                this.DataItem.Tank = value;
                this.RaisePropertyChanged(() => this.ShouldShowForCurrentTank);
                this.RaisePropertyChanged(() => this.Visible);
            }
        }

        public new DataItemViewDescriptor ViewDescriptor => (DataItemViewDescriptor)base.ViewDescriptor;

	    public override bool ShouldShowForCurrentTank => this.Tank == null ? false : this.ViewDescriptor.ShouldShowFor(this.Tank);

	    public override double DesiredHeight => 20;

	    public DataItemViewModel(DataItemViewDescriptor descriptor)
            : base(descriptor)
        {
            this.Name = App.GetLocalizedString(descriptor.NameL10NKey);
            this.FullName = App.GetLocalizedString(descriptor.FullNameL10NKey);
            this.DataItem = this.CreateDataItem(descriptor.Descriptor);
        }

        protected virtual DataItem CreateDataItem(DataItemDescriptorBase descriptor)
        {
            return new DataItem(descriptor);
        }

        public override void CompareTo(TankViewModelBase tank, bool? useInvertedComparation = null, DeltaValueDisplayMode? deltaValueDisplayMode = null)
        {
            this.DataItem.CompareTo(tank, useInvertedComparation, deltaValueDisplayMode);
        }

        public override void Dispose()
        {
            this.DataItem.Dispose();
        }

    }
}
