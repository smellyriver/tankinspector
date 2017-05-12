using Smellyriver.Serialization;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.IO
{
	internal static class Cache
    {
        private const string BinaryExtensionName = "bincache";
        private const string ImageExtensionName = "png";


        private static string GetCachePath(string folderName, string name, string extName)
        {
            var filename = $"{name}.{extName}";

            foreach (var invalidChar in Path.GetInvalidPathChars())
                folderName.Replace(invalidChar, '_');

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                filename.Replace(invalidChar, '_');

            return Path.Combine("Cache", Cache.GetApplicationVersion(), folderName, filename);
        }

        private static string GetCachePath(DatabaseKey databaseKey, string name, string extName, out string databaseFolderName)
        {
            databaseFolderName = databaseKey.HashString;

            return Cache.GetCachePath(databaseFolderName, name, extName);
        }

        private static string GetApplicationVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private static bool LoadCheck(string folderName, string name, string extName, out string filename)
        {
            filename = null;
            if (AppState.Default.FirstRun)
                return false;

            if (!Directory.Exists("Cache"))
                return false;

            if (!Directory.Exists(Path.Combine("Cache", Cache.GetApplicationVersion())))
                return false;

            filename = Cache.GetCachePath(folderName, name, extName);

            return File.Exists(filename);
        }

        private static bool LoadCheck(DatabaseKey databaseKey, string name, string extName, out string filename)
        {
            return Cache.LoadCheck(databaseKey.HashString, name, extName, out filename);
        }


        private static bool SaveCheck(string folderName, string name, string extName, out string filename)
        {
            try
            {
                var appVersion = Cache.GetApplicationVersion();

                using (Diagnostics.PotentialExceptionRegion)
                {
                    if (!Directory.Exists("Cache"))
                        Directory.CreateDirectory("Cache");

                    var appVersionPath = Path.Combine("Cache", appVersion);
                    if (!Directory.Exists(appVersionPath))
                        Directory.CreateDirectory(appVersionPath);

                    var versionPath = Path.Combine(appVersionPath, folderName);

                    if (!Directory.Exists(versionPath))
                        Directory.CreateDirectory(versionPath);
                }

                filename = Cache.GetCachePath(folderName, name, extName);

                return true;
            }
            catch (Exception)
            {
                filename = null;
                return false;
            }

        }

        private static bool SaveCheck(DatabaseKey databaseKey, string name, string extName, out string filename)
        {
            return Cache.SaveCheck(databaseKey.HashString, name, extName, out filename);
        }

        public static bool TryLoadBinary<TObject>(DatabaseKey databaseKey, string name, out TObject result)
        {
            return Cache.TryLoadBinary(databaseKey.HashString, name, out result);
        }

        public static bool TryLoadBinary<TObject>(string folderName, string name, out TObject result)
        {
            result = default(TObject);

			if (!Cache.LoadCheck(folderName, name, BinaryExtensionName, out string filename))
				return false;

			try
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    result = Serializer.BinaryDeserialize<TObject>(filename);
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Load binary cache '{name}' from '{folderName}' failed: {ex.ToString()}");
                try
                {
                    using (Diagnostics.PotentialExceptionRegion)
                    {
                        File.Delete(filename);
                    }
                }
                catch (Exception) { }
                return false;
            }
        }

        public static bool SaveBinary(DatabaseKey databaseKey, string name, object target)
        {
            return Cache.SaveBinary(databaseKey.HashString, name, target);
        }

        public static bool SaveBinary(string folderName, string name, object target)
        {
            try
            {
				if (!Cache.SaveCheck(folderName, name, BinaryExtensionName, out string filename))
					return false;

				using (Diagnostics.PotentialExceptionRegion)
                {
                    Serializer.BinarySerialize(target, filename);
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Save binary cache '{name}' to '{folderName}' failed: {ex.ToString()}");
                return false;
            }
        }

        public static bool TryLoadImage(DatabaseKey databaseKey, string name, out BitmapSource image)
        {
            image = null;

			if (!Cache.LoadCheck(databaseKey, name, ImageExtensionName, out string filename))
				return false;

			try
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    image = new BitmapImage(new Uri(Path.GetFullPath(filename), UriKind.Absolute));
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(
	                $"Load image cache '{name}' for version '{databaseKey.Version}' failed: {ex.ToString()}");
                return false;
            }
        }

        public static bool SaveImage(DatabaseKey databaseKey, string name, BitmapSource image)
        {
            try
            {
				if (!Cache.SaveCheck(databaseKey, name, ImageExtensionName, out string filename))
					return false;

				var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));

                using (Diagnostics.PotentialExceptionRegion)
                {
                    using (var stream = File.Create(filename))
                        encoder.Save(stream);
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(
	                $"Save image cache '{name}' for version '{databaseKey.Version}' failed: {ex.ToString()}");
                return false;
            }
        }

    }
}
