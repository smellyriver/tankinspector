using System.Collections.Specialized;
using System.ComponentModel;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class DetailedDataViewModel
    {

        private void HandleTankEvents()
        {
            if (this.Tank != null)
            {
                this.Tank.LoadedModules.PropertyChanged += TankLoadedModules_PropertyChanged;
                this.Tank.ModificationContext.PropertyChanged += TankModificationContext_PropertyChanged;
                this.Tank.LoadedEquipments.CollectionChanged += TankLoadedEquipments_CollectionChanged;
                this.Tank.LoadedConsumables.CollectionChanged += TankLoadedConsumables_CollectionChanged;
                this.Gun = this.Tank.LoadedModules.Gun;
            }
        }





        private void UnhandleTankEvents()
        {
            if (this.Tank != null)
            {
                this.Tank.LoadedModules.PropertyChanged -= TankLoadedModules_PropertyChanged;
                this.Tank.ModificationContext.PropertyChanged -= TankModificationContext_PropertyChanged; 
                this.Tank.LoadedEquipments.CollectionChanged -= TankLoadedEquipments_CollectionChanged;
                this.Tank.LoadedConsumables.CollectionChanged -= TankLoadedConsumables_CollectionChanged;
            }
        }

        private void HandleGunEvents()
        {
            if (this.Gun != null)
                this.Gun.PropertyChanged += Gun_PropertyChanged;
        }

        private void UnhandleGunEvents()
        {
            if (this.Gun != null)
                this.Gun.PropertyChanged -= Gun_PropertyChanged;
        }

	    private void Gun_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedShell")
            {
                this.NotifyDataItemValueChanges(DetailedDataRelatedComponentType.Shell);
            }
        }

	    private void TankLoadedModules_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DetailedDataRelatedComponentType relatedComponentType;

            switch (e.PropertyName)
            {
                case "Gun":
                    this.Gun = this.Tank.LoadedModules.Gun;
                    relatedComponentType = DetailedDataRelatedComponentType.Gun;
                    this.NotifyDataItemValueChanges(DetailedDataRelatedComponentType.Shell);
                    break;
                case "Turret":
                    relatedComponentType = DetailedDataRelatedComponentType.Turret; break;
                case "Chassis":
                    relatedComponentType = DetailedDataRelatedComponentType.Chassis; break;
                case "Engine":
                    relatedComponentType = DetailedDataRelatedComponentType.Engine; break;
                case "Radio":
                    relatedComponentType = DetailedDataRelatedComponentType.Radio; break;
                default:
                    return;
            }

            this.NotifyDataItemValueChanges(relatedComponentType);
        }

	    private void TankModificationContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyDataItemValueChanges(e.PropertyName);
        }

	    private void TankLoadedConsumables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyDataItemValueChanges(DetailedDataRelatedComponentType.Consumable);
        }

	    private void TankLoadedEquipments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyDataItemValueChanges(DetailedDataRelatedComponentType.Equipment);
        }
    }
}
