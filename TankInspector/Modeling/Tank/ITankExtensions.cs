namespace Smellyriver.TankInspector.Modeling
{
	internal static class TankExtensions 
    {

        public static uint GetTypeCompDescrId(this ITank tank)
        {
            return TypeCompDescr.Calculate(tank.TypeId, tank.NationId, tank.Id);
        }
    }
}
