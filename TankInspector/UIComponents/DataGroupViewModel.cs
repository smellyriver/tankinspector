using Smellyriver.TankInspector.DataAnalysis;
using System;
using System.Linq;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class DataGroupViewModel : IDisposable
    {
        public string Name { get; }
        public DataItemViewModelBase[] Items { get; }
        public bool ShowHeader { get; set; }

        private TankViewModelBase _tank;
        public TankViewModelBase Tank
        {
            get => _tank;
	        set
            {
                _tank = value;
                foreach (var item in this.Items)
                    item.Tank = _tank;
            }
        }


        public DataGroupViewModel(DataGroupViewDescriptor descriptor)
            : this(descriptor, App.GetLocalizedString(descriptor.NameL10NKey))
        {

        }

        public DataGroupViewModel(DataGroupViewDescriptor descriptor, string name)
        {
            this.Name = name;
            this.Items = descriptor.Items.Select(d => DataItemViewModelBase.Create(d)).ToArray();
            this.ShowHeader = true;
        }


        public void CompareTo(TankViewModelBase tank, bool? useInvertedComparation, DeltaValueDisplayMode? deltaValueDisplayMode = null)
        {
            foreach (var item in this.Items)
                item.CompareTo(tank, useInvertedComparation, deltaValueDisplayMode);
        }


        public void Dispose()
        {
            foreach (var item in this.Items)
                item.Dispose();
        }
    }
}
