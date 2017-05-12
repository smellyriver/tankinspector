namespace Smellyriver.TankInspector.Modeling
{
	internal static class TypeCompDescr
    {
        public static uint Calculate(uint typeId, uint nationId, uint tankId)
        {
            return (tankId << 8) + (nationId << 4) + typeId;
        }

        public static uint Calculate(uint nationId, uint tankId)
        {
            return TypeCompDescr.Calculate(1, nationId, tankId);
        }
    }
}
