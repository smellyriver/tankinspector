using log4net;
using Smellyriver.TankInspector.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Modeling
{
	internal class PackageDatabase : DatabaseObject, IPackageIndexer
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        [Serializable]
        private struct PackageIndexEntry
        {
            public readonly string FilePath;
            public readonly string PackagePath;

            public PackageIndexEntry(KeyValuePair<string, string> entry)
                : this()
            {
                this.FilePath = entry.Key;
                this.PackagePath = entry.Value;
            }
        }

        private Dictionary<string, string> _packageIndices;

        public PackageDatabase(Database database)
            : base(database)
        {
            this.LoadPackageIndices();
        }

        private void LoadPackageIndices()
        {

			if (Cache.TryLoadBinary(this.Database.Key, "package_indices", out PackageIndexEntry[] packageIndexEntries))
				_packageIndices = packageIndexEntries.ToDictionary(e => e.FilePath, e => e.PackagePath);
			else
			{
				Log.Info("load package indices from cache failed, gonna rebuild");
				this.BuildPackageIndices();

				Log.Info("saving package indices into cache");
				Cache.SaveBinary(this.Database.Key, "package_indices", _packageIndices.Select(p => new PackageIndexEntry(p)).ToArray());
			}
		}

        private void BuildPackageIndices()
        {
            _packageIndices = new Dictionary<string, string>();
            var packageRoot = Path.Combine(this.Database.RootPath, "res/packages");

            var paths = XElement.Load(Path.Combine(this.Database.RootPath, "paths.xml"));
            var packageFiles = paths.Descendants("Package").Select(p => p.Value)
                                                        .Where(p => p.EndsWith(".pkg"))
                                                        .Select(p => Path.Combine(this.Database.RootPath, p));

            foreach (var packageFile in packageFiles)
            {
                if (!File.Exists(packageFile))
                    continue;

                Log.InfoFormat("buiding package indices for '{0}'", packageFile);

                try
                {
                    foreach (var filePath in PackageStream.GetFileEntries(packageFile))
                        _packageIndices[filePath.ToLower()] = packageFile;
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("failed to build package indices for '{0}': '{1}'", packageFile, ex.Message);
                }
            }
        }

        public string GetPackagePath(string filename)
        {
	        if (_packageIndices.TryGetValue(filename.ToLower(), out string packagePath))
				return packagePath;
	        return null;
        }
    }
}
