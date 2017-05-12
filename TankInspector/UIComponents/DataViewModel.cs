using Smellyriver.TankInspector.DataAnalysis;
using System;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class DataViewModel : NotificationObject, IDisposable
    {
        public DataGroupViewModel DataGroup { get; }

        public TankViewModelBase Tank
        {
            get => this.DataGroup.Tank;
	        set => this.DataGroup.Tank = value;
        }

        public DeltaValueDisplayMode DeltaValueDisplayMode { get; set; }

        public bool ShowHeader
        {
            get => this.DataGroup.ShowHeader;
	        set => this.DataGroup.ShowHeader = value;
        }

        private bool _isDarken;
        public bool IsDarken
        {
            get => _isDarken;
	        set
            {
                _isDarken = value;
                this.RaisePropertyChanged(() => this.IsDarken);
            }
        }


        public DataViewModel(DataGroupViewDescriptor groupDescriptor, string name, IReferenceTankProvider comparedTankProvider)
        {
            this.DataGroup = new DataGroupViewModel(groupDescriptor, name);

            if (comparedTankProvider != null)
            {
                this.CompareTo(comparedTankProvider.ReferenceTank);
                comparedTankProvider.ReferenceTankChanged += OnComparedTankChanged;
            }
        }

        public DataViewModel(DataGroupViewDescriptor groupDescriptor, IReferenceTankProvider comparedTankProvider)
            : this(groupDescriptor, App.GetLocalizedString(groupDescriptor.NameL10NKey), comparedTankProvider)
        {

        }

        public DataViewModel(DataGroupViewDescriptor groupDescriptor)
            : this(groupDescriptor, App.GetLocalizedString(groupDescriptor.NameL10NKey), null)
        {

        }

        public DataViewModel()
        {

        }

	    private void OnComparedTankChanged(object sender, ReferenceTankChangedEventArgs e)
        {
            this.CompareTo(e.Tank);
        }

        public void CompareTo(TankViewModelBase tank, bool? useInvertedComparation = null)
        {
            this.DataGroup.CompareTo(tank, useInvertedComparation, this.DeltaValueDisplayMode);
        }

        public void Dispose()
        {
            this.DataGroup.Dispose();
        }
    }
}
