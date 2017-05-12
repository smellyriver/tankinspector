using ICSharpCode.SharpZipLib.Zip;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.IO
{
	internal class LoadingImage
    {
        private const string ImageRelativePath = "gui/maps/login";
        private const string ExcludedFile = "__login_bg.png";

        public static BitmapSource Random(string rootPath = null)
        {
            if (Database.Current == null)
                return null;

            if (rootPath == null)
                rootPath = Database.Current.RootPath;

            var packageFile = Path.Combine(Database.Current.RootPath, PackageImage.GuiPackage);

            using (var fileStream = File.OpenRead(packageFile))
            {
                using (var zipFile = new ZipFile(fileStream))
                {
                    var imageEntries = new List<ZipEntry>();

                    foreach (var entryObject in zipFile)
                    {
                        var entry = (ZipEntry)entryObject;
                        if (Path.GetDirectoryName(entry.Name).Replace('\\', '/') == ImageRelativePath && Path.GetFileName(entry.Name) != ExcludedFile)
                            imageEntries.Add(entry);
                    }

                    var selectedEntry = imageEntries[RandomInstances.Standard.Next(imageEntries.Count)];
                    using (var zipStream = zipFile.GetInputStream(selectedEntry))
                    {
                        return BitmapImageEx.FromStream(zipStream).ChangeDPI(96);
                    }
                }
            }

        }
    }
}
