using System.Collections.Generic;
using System.IO;
using Smellyriver.TankInspector.IO.MoDecoding;

namespace Smellyriver.TankInspector.Modeling
{
	internal class LocalizationDatabase : DatabaseObject
    {
        public string TextFolder { get; }

        private readonly Dictionary<string, MoData> _moDatum = new Dictionary<string, MoData>();

        public LocalizationDatabase(Database database, string textFolder)
            : base(database)
        {
            this.TextFolder = textFolder;
        }

        public string GetText(string key)
        {
            var info = TextDataInfo.Parse(key);

            if (info.BaseName == "") return info.MessageId;

            var dataBase = GetData(info.BaseName);

            if (dataBase == null)
                return $"$missing: {key}";

            return dataBase.Gettext(info.MessageId);
        }

        private MoData GetData(string baseName)
        {
			if (!_moDatum.TryGetValue(baseName, out MoData database))
			{
				var path = Path.Combine(TextFolder, baseName + ".mo");
				if (!File.Exists(path))
					return null;

				database = MoData.ReadFrom(path);

				_moDatum.Add(baseName, database);
			}
			return database;
        }

    }
}
