using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Smellyriver.Utilities;
using Smellyriver.Wpf.Utilities;

namespace Smellyriver.TankInspector.IO
{
	internal class PackageImage
    {

        public const string GuiPackage = "res/packages/gui.pkg";
        
        private static readonly Dictionary<string, BitmapSource> PackageImageCache;

        static PackageImage()
        {
            PackageImageCache = new Dictionary<string, BitmapSource>();
        }

        public static BitmapSource Load(string databaseRootPath, string package, string entryKey, double dpi = 96)
        {
            var packageFile = Path.Combine(databaseRootPath, package);
            var cacheKey = $"{packageFile}:{entryKey}";

            return PackageImageCache.GetOrCreate(cacheKey,
                () =>
                {
                    try
                    {
                        using (var stream = new PackageStream(packageFile, entryKey))
                        {
                            var image = BitmapImageEx.FromStream(stream);
                            if (image.DpiX != dpi || image.DpiY != dpi)
                                return image.ChangeDPI(dpi);
	                        return image;
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                });
        }


    }
}
