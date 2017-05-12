using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using System.Reflection;

namespace Smellyriver.TankInspector.IO
{
	internal class PackageStream : Stream
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);
        private static readonly Dictionary<string, ZipFile> SCachedZipFiles;
        private static readonly object SCachedZipFilesSyncObject = new object();

        static PackageStream()
        {
            ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            SCachedZipFiles = new Dictionary<string, ZipFile>();
        }

        private Stream _zipStream;


        private static ZipFile GetCachedZipFile(string packageFile)
        {
            if (!File.Exists(packageFile))
                return null;

            packageFile = PackageStream.NormalizePath(packageFile);

			if (!SCachedZipFiles.TryGetValue(packageFile, out ZipFile zipFile))
			{
				var stream = File.OpenRead(packageFile);

				zipFile = new ZipFile(stream);

				lock (SCachedZipFilesSyncObject)
					SCachedZipFiles[packageFile] = zipFile;

			}

			return zipFile;
        }

        public static bool IsFileExisted(string packageFile, string path)
        {
            if (!File.Exists(packageFile))
                return false;

            var zipFile = PackageStream.GetCachedZipFile(packageFile);
            return zipFile.FindEntry(path, true) != -1;
        }

        public static bool IsFileExisted(IPackageIndexer indexer, string path)
        {
            if (indexer == null)
                return false;

            return indexer.GetPackagePath(path) != null;
        }

        public static string[] GetFileEntries(string packageFile)
        {
            if (!File.Exists(packageFile))
                return null;

            var zipFile = PackageStream.GetCachedZipFile(packageFile);
            return zipFile.Cast<ZipEntry>().Select(e => e.Name).ToArray();
        }

        public static string NormalizePath(string path)
        {
            return path == null ? null : path.Replace('\\', '/').ToLower();
        }

        public PackageStream(IPackageIndexer indexer, string path)
            : this(indexer.GetPackagePath(path), path)
        {

        }

        public PackageStream(string packageFile, string path)
        {
            Log.InfoFormat("open package file {0} from {1}", path, packageFile);
            var zipFile = PackageStream.GetCachedZipFile(PackageStream.NormalizePath(packageFile));
            try
            {
                var entryIndex = zipFile.FindEntry(path, true);
                _zipStream = zipFile.GetInputStream(entryIndex);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"cannot find {path} in {packageFile}");
            }
        }

        public override bool CanRead => _zipStream.CanRead;

	    public override bool CanSeek => _zipStream.CanSeek;

	    public override bool CanWrite => _zipStream.CanWrite;

	    public override void Flush()
        {
            _zipStream.Flush();
        }

        public override long Length => _zipStream.Length;

	    public override long Position
        {
            get => _zipStream.Position;
		    set => _zipStream.Position = value;
	    }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _zipStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _zipStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _zipStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _zipStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_zipStream != null)
                {
                    _zipStream.Dispose();
                    _zipStream = null;
                }
            }
        }

    }
}
