namespace Smellyriver.TankInspector.DataAnalysis
{
	internal class DataGroupViewDescriptor : IDataViewDescriptor
    {
        public string NameL10NKey { get; }

        public DataItemViewDescriptorCollection Items { get; }

        public DataGroupViewDescriptor(string nameL10NKey)
        {
            this.NameL10NKey = nameL10NKey;
            this.Items = new DataItemViewDescriptorCollection(this);
        }
    }
}
