using Smellyriver.TankInspector.Modeling;
using System.IO;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.IO
{
	internal class ShellIcon
    {
        private const string IconRelativePath = "gui/maps/icons/shell";


        public static BitmapSource Load(string databaseRootPath, ShellType shellType, double dpi = 96)
        {
            string key;

            switch (shellType)
            {
                case ShellType.AP:
                    key = "ARMOR_PIERCING";
                    break;
                case ShellType.APCR:
                    key = "ARMOR_PIERCING_CR";
                    break;
                case ShellType.APHE:
                    key = "ARMOR_PIERCING_HE";
                    break;
                case ShellType.HE:
                    key = "HIGH_EXPLOSIVE";
                    break;
                case ShellType.PremiumHE:
                    key = "HIGH_EXPLOSIVE_PREMIUM";
                    break;
                case ShellType.HEAT:
                    key = "HOLLOW_CHARGE";
                    break;
                default:
                    return null;
            }

            var entryKey = Path.Combine(IconRelativePath, key + ".png").Replace('\\', '/');

            return PackageImage.Load(databaseRootPath, PackageImage.GuiPackage, entryKey);

        }
    }
}
