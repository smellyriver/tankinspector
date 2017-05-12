using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class FuelTypeViewModel
    {

        public FuelType FuelType { get; }
        public FuelTypeViewModel(FuelType fuelType)
        {
            this.FuelType = fuelType;
        }

        public override string ToString()
        {
            switch (this.FuelType)
            {
                case FuelType.Diesel:
                    return App.GetLocalizedString("Diesel");
                case FuelType.Gasoline:
                    return App.GetLocalizedString("Gasoline");
                default:
                    return this.FuelType.ToString();
            }
        }
    }
}
