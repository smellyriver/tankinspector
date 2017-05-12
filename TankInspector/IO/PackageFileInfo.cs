namespace Smellyriver.TankInspector.IO
{
	internal class PackageFileInfo
    {
        public PackageFileInfo(IPackageIndexer databasePackageIndexer, string fileInPackagePath)
        {
            PackageIndexer = databasePackageIndexer;
            FileInPackagePath = fileInPackagePath;        
        }

        public IPackageIndexer PackageIndexer { get; set; }
        public string FileInPackagePath { get; set; }
    }
}
