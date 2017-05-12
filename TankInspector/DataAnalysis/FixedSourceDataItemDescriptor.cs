using Smellyriver.TankInspector.UIComponents;
using System;
using System.ComponentModel;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal class FixedSourceDataItemDescriptor<TDataSource> : DataItemDescriptorBase
    {
        private readonly DataItemDescriptor<TDataSource> _dataItemDescriptor;
        private readonly TDataSource _dataSource;


        public override string FullNameL10NKey => _dataItemDescriptor.FullNameL10NKey;

	    private string _unit;
        public override string Unit => _unit;

	    public override double BenchmarkRatio => _dataItemDescriptor.BenchmarkRatio;

	    public override string Description => _dataItemDescriptor.Description;

	    public override string Format => _dataItemDescriptor.Format;

	    public override bool IsDecimal => _dataItemDescriptor.IsDecimal;

	    public override ComparisonMode ComparationMode => _dataItemDescriptor.ComparationMode;
		
	    private readonly Func<TDataSource, object> _valueRetriever;

        private readonly TankViewModelBase _dataSourceOwner;

        public FixedSourceDataItemDescriptor(DataItemDescriptor<TDataSource> dataItemDescriptor, TankViewModelBase dataSourceOwner, TDataSource dataSource, Func<TDataSource, object> valueRetriever)
        {
            _dataItemDescriptor = dataItemDescriptor;
            _dataSourceOwner = dataSourceOwner;

            _unit = _dataItemDescriptor.Unit;

            _dataSource = dataSource;
            _valueRetriever = valueRetriever;

            _dataItemDescriptor.PropertyChanged += _dataItemDescriptor_PropertyChanged;
        }

	    private void _dataItemDescriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }


        protected override object GetRawValue(TankViewModelBase tank, bool updateRelatedFields = false)
        {
            if (tank == null)
                return null;

            TDataSource dataSource;
            if (tank != _dataSourceOwner)
                dataSource = _dataItemDescriptor.DefaultDataSourceRetriever(tank);
            else
                dataSource = _dataSource;

            var value = _valueRetriever(dataSource);
            if (updateRelatedFields && value is IUnitProvider)
            {
                _unit = ((IUnitProvider)value).Unit;
                this.RaisePropertyChanged(() => this.Unit);
            }

            return value;
        }


    }
}
