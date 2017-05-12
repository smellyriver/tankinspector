using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.UIComponents
{
    class ComplexDataItemHeaderViewModel : DataItemViewModelBase
    {

        public string Name { get { return _owner.Name; } }

        public bool IsDarken
        {
            get { return _owner.IsDarken; }
            internal set
            {
                _owner.IsDarken = value;
                this.RaisePropertyChanged(() => this.IsDarken);
            }
        }

        public override double DesiredHeight
        {
            get { return _owner.Items.Where(i => i.IsPrioritySufficient).Any() ? 20 : 0; }
        }

        public override int Priority
        {
            get { return _owner.Priority; }
        }

        private ComplexDataItemViewModel _owner;

        public ComplexDataItemHeaderViewModel(ComplexDataItemViewModel owner)
            : base(null, null)
        {
            _owner = owner;
        }

        public override void CompareTo(TankViewModelBase tank, bool? useInvertedComparation = null, DeltaValueDisplayMode? deltaValueDisplayMode = null)
        {
            // do nothing
        }
    }
}
