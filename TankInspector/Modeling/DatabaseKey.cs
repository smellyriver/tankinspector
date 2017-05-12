using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Modeling
{
    [DebuggerDisplay("{Version}")]
    [Serializable]
    public struct DatabaseKey
    {
        private static readonly MD5 SMd5 = MD5.Create();

        public string RootPath { get; set; }
        public string Version { get; set; }

        private string IdentifierString => this.RootPath + "|" + this.Version;

	    public string HashString => SMd5.Hash(this.IdentifierString);

	    public override bool Equals(object obj)
        {
            if (!(obj is DatabaseKey))
                return false;

            var other = (DatabaseKey)obj;
            return other.RootPath == this.RootPath
                && other.Version == this.Version;
        }

        public override int GetHashCode()
        {
            return this.IdentifierString.GetHashCode();
        }

        public DatabaseKey(string rootPath, string version)
            : this()
        {
            this.RootPath = rootPath;
            this.Version = version;
        }
    }
}
