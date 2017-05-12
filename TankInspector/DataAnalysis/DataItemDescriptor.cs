using Smellyriver.TankInspector.UIComponents;
using System;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal class DataItemDescriptor<TDataSource> : DataItemDescriptorBase
    {
	    public override string FullNameL10NKey { get; }

	    private string _unit;
        public override string Unit => _unit;

	    public override double BenchmarkRatio { get; }

	    public override bool IsDecimal { get; }

	    public Func<TankViewModelBase, TDataSource> DefaultDataSourceRetriever { get; }

        protected Func<TDataSource, object> ValueRetriever { get; }

	    public override string Description { get; }

	    public override string Format { get; }

	    public override ComparisonMode ComparationMode { get; }

	    public DataItemDescriptor(string fullNameL10NKey, Func<TankViewModelBase, TDataSource> defaultDataSourceRetriver, Func<TDataSource, object> valueRetriever,
			string unitL10NKey, string descriptionL10NKey, string format = "{0}", ComparisonMode comparationMode = ComparisonMode.HigherBetter,
			double benchmarkRatio = 0.25, bool isDecimal = true)
		{
            this.FullNameL10NKey = fullNameL10NKey;
            this.DefaultDataSourceRetriever = defaultDataSourceRetriver;
            this.ValueRetriever = valueRetriever;
            _unit = App.GetLocalizedString(unitL10NKey);
            this.Format = format;
            this.Description = App.GetLocalizedString(descriptionL10NKey);
            this.ComparationMode = comparationMode;
            this.BenchmarkRatio = benchmarkRatio;
            this.IsDecimal = isDecimal;
        }

        public FixedSourceDataItemDescriptor<TDataSource> SpecifySource(TankViewModelBase dataSourceOwner, TDataSource dataSource)
        {
            return new FixedSourceDataItemDescriptor<TDataSource>(this, dataSourceOwner, dataSource, this.ValueRetriever);
        }

        protected virtual TDataSource GetDataSource(TankViewModelBase tank)
        {
            return this.DefaultDataSourceRetriever(tank);
        }

        protected override object GetRawValue(TankViewModelBase tank, bool updateRelatedFields = false)
        {
            if (tank == null)
                return null;

            var value = this.ValueRetriever(this.GetDataSource(tank));
            if (updateRelatedFields && value is IUnitProvider)
            {
                _unit = ((IUnitProvider)value).Unit;
                this.RaisePropertyChanged(() => this.Unit);
            }

            return value;
        }


    }
}
