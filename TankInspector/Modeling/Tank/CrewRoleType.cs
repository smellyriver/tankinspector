using System;

namespace Smellyriver.TankInspector.Modeling
{
	internal enum CrewRoleType
    {
        All,
        Commander,
        Driver,
        Gunner,
        Loader,
        Radioman,
    }

	internal static class CrewRoleTypeEx
    {

        public static CrewRoleType Parse(string role)
        {
            switch (role)
            {
                case "commander":
                    return CrewRoleType.Commander;
                case "gunner":
                    return CrewRoleType.Gunner;
                case "loader":
                    return CrewRoleType.Loader;
                case "driver":
                    return CrewRoleType.Driver;
                case "radioman":
                    return CrewRoleType.Radioman;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
