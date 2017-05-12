using System.Collections.ObjectModel;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal class DataItemViewDescriptorCollection : Collection<DataItemViewDescriptorBase>
    {
        private readonly IDataViewDescriptor _owner;
        public DataItemViewDescriptorCollection(IDataViewDescriptor owner)
        {
            _owner = owner;
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.Parent = null;
            }

            base.ClearItems();
        }

        protected override void InsertItem(int index, DataItemViewDescriptorBase item)
        {
            item.Parent = _owner;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            item.Parent = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, DataItemViewDescriptorBase item)
        {
            item.Parent = _owner;
            base.SetItem(index, item);
        }
    }
}
