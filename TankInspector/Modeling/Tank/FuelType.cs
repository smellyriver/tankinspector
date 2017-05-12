using System;

namespace Smellyriver.TankInspector.Modeling
{
	internal enum FuelType
    {
        Gasoline,
        Diesel
    }

	internal static class FuelTypeEx
    {
        public static FuelType Parse(string fuelType)
        {
            switch (fuelType.Trim())
            {
                case "gasoline":
                    return FuelType.Gasoline;
                case "diesel":
                    return FuelType.Diesel;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
