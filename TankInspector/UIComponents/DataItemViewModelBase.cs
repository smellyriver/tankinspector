using Smellyriver.TankInspector.DataAnalysis;
using System;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class DataItemViewModelBase : NotificationObject, IPriorityItem, IDisposable
	{

		public static DataItemViewModelBase Create(DataItemViewDescriptorBase descriptor)
		{
			var itemViewDescriptor = descriptor as ComplexDataItemViewDescriptor;
			if (itemViewDescriptor != null)
				return new ComplexDataItemViewModel(itemViewDescriptor);

			var viewDescriptor = descriptor as DataItemViewDescriptor;
			if (viewDescriptor != null)
			{
				var dataItemViewDescriptor = viewDescriptor;

				return new DataItemViewModel(dataItemViewDescriptor);
			}

			var separatorDescriptor = descriptor as DataSeparatorDescriptor;
			if (separatorDescriptor != null)
				return new DataSeparatorViewModel(separatorDescriptor);

			throw new NotSupportedException();
		}

		public DataItemViewDescriptorBase ViewDescriptor { get; }

		private TankViewModelBase _tank;
		public virtual TankViewModelBase Tank
		{
			get => _tank;
			set => _tank = value ?? throw new ArgumentNullException(nameof(value));
		}

		public int Priority => ViewDescriptor.GetPriority(this.Tank);

		private bool _isPrioritySufficient;
		public bool IsPrioritySufficient
		{
			get => _isPrioritySufficient;
			set
			{
				_isPrioritySufficient = value;
				this.RaisePropertyChanged(() => this.IsPrioritySufficient);
				this.RaisePropertyChanged(() => this.Visible);
			}
		}

		public virtual bool ShouldShowForCurrentTank => true;

		public bool Visible => this.IsPrioritySufficient && this.ShouldShowForCurrentTank;

		public abstract double DesiredHeight { get; }

		public DataItemViewModelBase(DataItemViewDescriptorBase descriptor)
		{
			ViewDescriptor = descriptor;
		}

		public abstract void CompareTo(TankViewModelBase tank, bool? useInvertedComparation = null, DeltaValueDisplayMode? deltaValueDisplayMode = null);

		public virtual void Dispose()
		{

		}

	}
}
