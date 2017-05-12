using Smellyriver.TankInspector.DataAnalysis;
using System.Linq;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class ComplexDataItemViewModel : DataItemViewModelBase
    {
        public string Name { get; }
        public DataItemViewModelBase[] Items { get; }

        private bool _isDarken;
        public bool IsDarken
        {
            get => _isDarken;
	        internal set
            {
                _isDarken = value;
                this.RaisePropertyChanged(() => this.IsDarken);
            }
        }

        public override TankViewModelBase Tank
        {
            get => base.Tank;
	        set
            {
                base.Tank = value;
                foreach (var item in this.Items)
                    item.Tank = base.Tank;
            }
        }

        public override double DesiredHeight
        {
            get
            {
                if (this.IsPrioritySufficient)
                {
                    var childDesiredHeight = this.Items.Where(i => i.IsPrioritySufficient).Sum(i => i.DesiredHeight);
                    if (childDesiredHeight > 0)
                        return childDesiredHeight + 20;
                }
                return 0;
            }
        }

        public ComplexDataItemViewModel(ComplexDataItemViewDescriptor descriptor)
            : base(descriptor)
        {
            this.Name = App.GetLocalizedString(descriptor.NameL10NKey);
            this.Items = descriptor.Items.Select(d =>
                {
                    var vm = DataItemViewModelBase.Create(d);
                    if (vm is DataItemViewModel)
                        ((DataItemViewModel)vm).IsSubItem = true;

                    return vm;
                }).ToArray();
        }

        public override void CompareTo(TankViewModelBase tank, bool? useInvertedComparation = null, DeltaValueDisplayMode? deltaValueDisplayMode = null)
        {
            foreach (var item in this.Items)
                item.CompareTo(tank, useInvertedComparation, deltaValueDisplayMode);
        }

        public override void Dispose()
        {
            foreach (var item in this.Items)
                item.Dispose();
        }
    }
}
