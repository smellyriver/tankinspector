using System;

namespace Smellyriver.TankInspector.Modeling
{
    public enum ShellType
    {
        Ap,
        Apcr,
        He,
        PremiumHe,
        Heat,
        Aphe
    }

	internal static class ShellTypeExtensions
    {
        public static bool IsKineticShellType(this ShellType type)
        {
            switch (type)
            {
                case ShellType.He:
                case ShellType.PremiumHe:
                    return false;
                case ShellType.Ap:
                case ShellType.Apcr:
                case ShellType.Aphe:
                case ShellType.Heat:
                    return true;
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool HasNormalizationEffect(this ShellType type)
        {
            switch (type)
            {
                case ShellType.He:
                case ShellType.PremiumHe:
                case ShellType.Heat:
                    return false;
                case ShellType.Ap:
                case ShellType.Apcr:
                case ShellType.Aphe:          
                    return true;
                default:
                    throw new NotSupportedException();
            }
        }

        public static double BasicNormalization(this ShellType type)
        {
            switch (type)
            {
                case ShellType.He:
                case ShellType.PremiumHe:
                case ShellType.Heat:
                    return 0.0;
                case ShellType.Ap:
                case ShellType.Aphe:
                    return 5.0;
                case ShellType.Apcr:
                    return 2.0;
                default:
                    throw new NotSupportedException();
            }
        }

        public static double RicochetAngle(this ShellType type)
        {
            switch (type)
            {
                case ShellType.He:  
                case ShellType.PremiumHe:
                    return 90.0;
                case ShellType.Heat:
                    return 85.0;
                case ShellType.Ap:
                case ShellType.Aphe:
                case ShellType.Apcr:
                    return 70.0;
                default:
                    throw new NotSupportedException();
            }
        }        
    }
}
