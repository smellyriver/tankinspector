using System;
using System.Diagnostics.CodeAnalysis;

namespace Smellyriver.TankInspector.Modeling
{
    public enum ShellType
    {
        AP,
        APCR,
        HE,
        PremiumHE,
        HEAT,
        APHE,
        SMOKE
    }

	internal static class ShellTypeExtensions
    {
        public static bool IsKineticShellType(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                case ShellType.PremiumHE:
                case ShellType.SMOKE:
                    return false;
                case ShellType.AP:
                case ShellType.APCR:
                case ShellType.APHE:
                case ShellType.HEAT:
                    return true;
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool HasNormalizationEffect(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                case ShellType.PremiumHE:
                case ShellType.HEAT:
                case ShellType.SMOKE:
                    return false;
                case ShellType.AP:
                case ShellType.APCR:
                case ShellType.APHE:          
                    return true;
                default:
                    throw new NotSupportedException();
            }
        }

        public static double BasicNormalization(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                case ShellType.PremiumHE:
                case ShellType.HEAT:
                case ShellType.SMOKE:
                    return 0.0;
                case ShellType.AP:
                case ShellType.APHE:
                    return 5.0;
                case ShellType.APCR:
                    return 2.0;
                default:
                    throw new NotSupportedException();
            }
        }

        public static double RicochetAngle(this ShellType type)
        {
            switch (type)
            {
                case ShellType.SMOKE:
                    return 0.0;
                case ShellType.HE:  
                case ShellType.PremiumHE:
                    return 90.0;
                case ShellType.HEAT:
                    return 85.0;
                case ShellType.AP:
                case ShellType.APHE:
                case ShellType.APCR:
                    return 70.0;
                default:
                    throw new NotSupportedException();
            }
        }        
    }
}
