using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class DetailedDataViewModel : NotificationObject
    {

        private readonly Dispatcher _dispatcher;


        private GunViewModel _gun;
        private GunViewModel Gun
        {
            get => _gun;
	        set
            {
                this.UnhandleGunEvents();
                _gun = value;
                this.HandleGunEvents();
            }
        }


        private TankViewModel _tank;
        public TankViewModel Tank
        {
            get { return _tank; }
            set
            {
                if (_tank != value)
                {
                    if (_tank != null)
                        this.DisposeTankData();

                    _tank = value;
                    this.RaisePropertyChanged(() => this.Tank);

                    this.InitializeTankData();
                }
            }
        }

        public int MinimumDataPriority => ApplicationSettings.Default.ShowAsMuchStatsAsPossible ? -1000 : -100;


	    private void DisposeTankData()
        {
            foreach (var dataVm in _allDataViewModels)
                dataVm.Dispose();

            this.UnhandleTankEvents();
        }

        private void InitializeTankData()
        {
            foreach (var dataGroup in _allDataViewModels)
                dataGroup.Tank = this.Tank;

            this.HandleTankEvents();
            this.CompareWithReferenceTank();
        }

        private void NotifyDataItemValueChanges(DetailedDataRelatedComponentType relatedComponentType)
        {
			if (_dataItemComponentRelationships.TryGetValue(relatedComponentType, out List<DataItem> dataItems))
			{
				foreach (var dataItem in dataItems)
					dataItem.NotifyValueChanged();
			}
		}

        private void NotifyDataItemValueChanges(string relatedModificationDomain)
        {
			if (_dataItemModificationDomainRelationships.TryGetValue(relatedModificationDomain, out List<DataItem> dataItems))
			{
				foreach (var dataItem in dataItems)
					dataItem.NotifyValueChanged();
			}
		}

        private void ResetDarkenStates()
        {
            foreach (var dataItem in _allDataItems)
                dataItem.IsDarken = false;
            foreach (var complexVM in _allComplexDataItemVMs)
                complexVM.IsDarken = false;
        }

        private void HighlightDataItems(IEnumerable<DataItem> dataItems)
        {
            if (dataItems == null)
            {
                foreach (var dataItem in _allDataItems)
                    dataItem.IsDarken = true;

                foreach (var complexVM in _allComplexDataItemVMs)
                    complexVM.IsDarken = true;
            }
            else
            {
                var highlightedDataItems = new HashSet<DataItem>(dataItems);
                foreach (var dataItem in _allDataItems)
                    dataItem.IsDarken = !highlightedDataItems.Contains(dataItem);

                foreach (var complexVM in _allComplexDataItemVMs)
                    complexVM.IsDarken = complexVM.Items.OfType<DataItemViewModel>().All(item => item.DataItem.IsDarken);
            }
        }

        private void HighlightDataItemsWithDeltaValue()
        {
            var highlightedDataItems = new HashSet<DataItem>();
            foreach (var dataItem in _allDataItems)
            {
                if (Math.Abs(dataItem.DeltaRatio) > double.Epsilon)
                {
                    dataItem.IsDarken = false;
                    highlightedDataItems.Add(dataItem);
                }
                else
                    dataItem.IsDarken = true;
            }

            foreach (var complexVM in _allComplexDataItemVMs)
                complexVM.IsDarken = complexVM.Items.OfType<DataItemViewModel>().All(item => item.DataItem.IsDarken);
        }


        private DetailedDataRelatedComponentType _highlightedComponentType;
        public DetailedDataRelatedComponentType HighlightedComponentType
        {
            get => _highlightedComponentType;
	        set
            {
                _highlightedComponentType = value;

                // this will be handled by HighlightedModificationDomains
                if ((_highlightedComponentType & DetailedDataRelatedComponentType.HasModificationEffects) == DetailedDataRelatedComponentType.HasModificationEffects)
                    return;

                if (_highlightedComponentType == DetailedDataRelatedComponentType.HasDeltaValue)
                    return;

                if (value == DetailedDataRelatedComponentType.None)
                    this.ResetDarkenStates();
                else
                {
					_dataItemComponentRelationships.TryGetValue(value, out List<DataItem> dataItems);
					this.HighlightDataItems(dataItems);
                }
            }
        }

        private IEnumerable<string> _highlightedModificationDomain;
        public IEnumerable<string> HighlightedModificationDomains
        {
            get => _highlightedModificationDomain;
	        set
            {
                _highlightedModificationDomain = value;

                if ((this.HighlightedComponentType & DetailedDataRelatedComponentType.HasModificationEffects) == DetailedDataRelatedComponentType.HasModificationEffects)
                {

                    IEnumerable<DataItem> dataItems = null;
					if (_dataItemComponentRelationships.TryGetValue(this.HighlightedComponentType, out List<DataItem> dataItemList))
						dataItems = dataItemList;

					if (value != null)
                    {
                        foreach (var domain in value)
                        {

                            if (_dataItemModificationDomainRelationships.TryGetValue(domain, out dataItemList))
                            {
                                if (dataItems == null)
                                    dataItems = dataItemList;
                                else
                                    dataItems = dataItems.Union(dataItemList);
                            }
                        }
                    }

                    this.HighlightDataItems(dataItems);
                }
            }
        }

        public DetailedDataViewModel(TankViewModel tank, IReferenceTankProvider constantComparedTankProvider)
        {
            this.InitializeDataGroups();

            this.Tank = tank;
            _dispatcher = Dispatcher.CurrentDispatcher;

            constantComparedTankProvider.ReferenceTankChanged += OnConstantComparedTankChanged;
            this.SetConstantComparedTank(constantComparedTankProvider.ReferenceTank);

            
        }


	    private void OnConstantComparedTankChanged(object sender, ReferenceTankChangedEventArgs e)
        {
            this.SetConstantComparedTank(e.Tank);
        }

	    private void SetConstantComparedTank(TankViewModelBase tank)
        {
            if (_referenceTank != tank)
            {
                _referenceTank = tank;
                if (tank != null && !this.IsComparing)
                    this.CompareWithReferenceTank();
            }
        }


    }
}
