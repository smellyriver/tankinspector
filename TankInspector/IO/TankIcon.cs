using System.IO;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.IO
{
	internal class TankIcon
    {
        public const string VirtualTankIconKey = "__VIRTUAL";

        private const string IconRelativePath = "gui/maps/icons/vehicle";
        private const string NormalPath = "";
        private const string ContourPath = "contour";
        private const string SmallPath = "small";


        public static bool Exists(string databaseRootPath, string hyphenKey, TankIconType type = TankIconType.Normal)
        {
            if (hyphenKey == VirtualTankIconKey)
                return true;

            return PackageStream.IsFileExisted(Path.Combine(databaseRootPath, PackageImage.GuiPackage), TankIcon.GetEntryKey(hyphenKey, type));
        }

        private static string GetEntryKey(string hyphenKey, TankIconType type)
        {
            return Path.Combine(IconRelativePath, TankIcon.GetVehicleIconSubpath(type), hyphenKey + ".png").Replace('\\', '/');
        }

        public static BitmapSource Load(string databaseRootPath, string hyphenKey, double dpi = 96, TankIconType type = TankIconType.Normal)
        {

            if (hyphenKey == VirtualTankIconKey)
                hyphenKey = "ussr-Observer";

            var path = TankIcon.GetEntryKey(hyphenKey, type);
            if (PackageStream.IsFileExisted(Path.Combine(databaseRootPath, PackageImage.GuiPackage), path))
                return PackageImage.Load(databaseRootPath, PackageImage.GuiPackage, path);
	        return PackageImage.Load(databaseRootPath, PackageImage.GuiPackage, TankIcon.GetEntryKey("noImage", type));
        }

        private static string GetVehicleIconSubpath(TankIconType type)
        {
            switch (type)
            {
                case TankIconType.Contour:
                    return ContourPath;
                case TankIconType.Small:
                    return SmallPath;
                case TankIconType.Normal:
                    return NormalPath;
                default:
                    return "";
            }
        }
    }
}
