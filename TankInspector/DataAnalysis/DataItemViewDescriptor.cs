using Smellyriver.TankInspector.UIComponents;
using System;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal class DataItemViewDescriptor : DataItemViewDescriptorBase
    {
        private static readonly Predicate<TankViewModelBase> AlwaysTrue = t => true;

        public string NameL10NKey { get; }

        public string FullNameL10NKey => this.Descriptor.FullNameL10NKey;

	    public DataItemDescriptorBase Descriptor { get; }

        public Func<TankViewModelBase, int> PrioritySelector { get; set; }

        private int _priority;
        public int Priority
        {
            get
            {
                if (this.PrioritySelector != null)
                    throw new InvalidOperationException("a priority selector is present, this property is disabled");
                return _priority;
            }
            set
            {
                if (this.PrioritySelector != null)
                    throw new InvalidOperationException("a priority selector is present, this property is disabled");
                _priority = value;
            }
        }


        private Predicate<TankViewModelBase> _showCondition;

        public Predicate<TankViewModelBase> ShowCondition
        {
            get { return _showCondition; }
            set
            {
                if (value == null)
                    _showCondition = AlwaysTrue;
                else
                    _showCondition = value;
            }
        }


        public DataItemViewDescriptor(string nameL10NKey, DataItemDescriptorBase descriptor, int priority = 0, Predicate<TankViewModelBase> showCondition = null)
        {
            this.NameL10NKey = nameL10NKey;
            this.Descriptor = descriptor;
            this.Priority = priority;
            this.ShowCondition = showCondition;
        }

        public bool ShouldShowFor(TankViewModelBase tank)
        {
            return this.ShowCondition(tank);
        }

        public override int GetPriority(TankViewModelBase tank)
        {
	        if (this.PrioritySelector != null)
                return this.PrioritySelector(tank);
	        return _priority;
        }
    }
}
