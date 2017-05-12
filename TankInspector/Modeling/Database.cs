using Smellyriver.TankInspector.IO.XmlDecoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Smellyriver.Collection;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Design;
using System.Text.RegularExpressions;
using System.Reflection;
using log4net;

namespace Smellyriver.TankInspector.Modeling
{
	internal partial class Database
    {

        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        public const int MaxTankTier = 10;

        public string Version { get; private set; }

        internal static bool ParseVersion(string version, out uint major, out uint minor, out uint build, out uint revision, out bool isCommonTest)
        {
            var match = Regex.Match(version, @"^\s*v\.(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+) (?<ct>Common Test )?\#(?<revision>\d+)\s*$");
            if (match.Success)
            {
                if (uint.TryParse(match.Groups["major"].Value, out major))
                    if (uint.TryParse(match.Groups["minor"].Value, out minor))
                        if (uint.TryParse(match.Groups["build"].Value, out build))
                            if (uint.TryParse(match.Groups["revision"].Value, out revision))
                            {
                                if (string.IsNullOrEmpty(match.Groups["ct"].Value))
                                    isCommonTest = false;
                                else
                                    isCommonTest = true;

                                return true;
                            }
            }

            major = minor = build = revision = 0;
            isCommonTest = false;

            return false;
        }

        public uint VersionId
        {
            get
            {
				if (Database.ParseVersion(this.Version, out uint major, out uint minor, out uint build, out uint revision, out bool isCommonTest))
					return major * 10000 + minor * 100 + build;

				return 0;
            }
        }


        public DatabaseKey Key { get; private set; }

        private const string ScriptsPackageFile = @"res\packages\scripts.pkg";
        public const string VehiclesFolderInPackage = @"scripts/item_defs/vehicles/";
        private const string VersionFile = @"version.xml";
        private const string TextFolder = @"res\text\LC_MESSAGES";
        private const string CommonVehicleDataFile = @"scripts/item_defs/vehicles/common/vehicle.xml";
        private const string EquipmentDataFile = @"scripts/item_defs/vehicles/common/optional_devices.xml";
        private const string ConsumableDataFile = @"scripts/item_defs/vehicles/common/equipments.xml";
        private const string CrewSkillDataFile = @"scripts/item_defs/tankmen/tankmen.xml";
        internal static bool IsPathValid(string rootPath)
        {
            return File.Exists(Path.Combine(rootPath, VersionFile))
                   && Directory.Exists(Path.Combine(rootPath, TextFolder))
                   && File.Exists(Path.Combine(rootPath, ScriptsPackageFile));

        }

        public LazyDictionary<string, NationalDatabase> Nations { get; }

        public CommonVehicleDatabase CommonVehicleData { get; private set; }

        public EquipmentDatabase EquipmentDatabase { get; private set; }

        public ConsumableDatabase ConsumableDatabase { get; private set; }

        public CrewDatabase CrewDatabase { get; private set; }

        public PackageDatabase PackageDatabase { get; private set; }

        public Dictionary<int, Dictionary<TankClass, BenchmarkTank>> BenchmarkTanks { get; private set; }
        private Dictionary<string, BenchmarkTank> _benchmarkTankDict;
        public IEnumerable<Tank> Tanks
        {
            get
            {
                foreach (var nation in this.Nations.Values)
                    foreach (var tank in nation.Tanks.Values)
                        yield return tank;
            }
        }

        public LocalizationDatabase TextData { get; }

        public string RootPath { get; }

        public bool IsFullyLoaded { get; private set; }

        public Database(string rootPath)
        {
            if (!Database.IsPathValid(rootPath))
                throw new ArgumentException("invalid root nationName");

            RootPath = rootPath;
            TextData = new LocalizationDatabase(this, Path.Combine(RootPath, TextFolder));
            this.Nations = new LazyDictionary<string, NationalDatabase>();

            this.ReadVersion();
        }



        public ITank GetTank(string colonFullKey)
        {
            var keyParts = colonFullKey.Split(new[] { ':' }, 2);
            if (keyParts.Length != 2)
                return null;

            if (keyParts[0] == BenchmarkTank.DefaultNationKey)
            {
	            if (_benchmarkTankDict.TryGetValue(keyParts[1], out BenchmarkTank benchmarkTank))
					return benchmarkTank;
	            return null;
            }

			if (!this.Nations.TryGetValue(keyParts[0], out NationalDatabase nation))
				return null;

			if (!nation.Tanks.TryGetValue(keyParts[1], out Tank tank))
				return null;

			return tank;
        }

        public ITank GetTank(uint compDescr)
        {
            return this.Tanks.FirstOrDefault(t => t.GetTypeCompDescrId() == compDescr);
        }

        public BenchmarkTank GetBenchmarkTank(int tier, TankClass @class)
        {
			if (this.BenchmarkTanks.TryGetValue(tier, out Dictionary<TankClass, BenchmarkTank> classBenchmarks))
			{
				if (classBenchmarks.TryGetValue(@class, out BenchmarkTank benchmark))
					return benchmark;
			}

			return null;
        }

        public void FullyLoad()
        {

            Log.InfoFormat("fully loading database: version='{0}', nationName='{1}'", this.Version, this.RootPath);

            if (this.IsFullyLoaded)
            {
                Log.Info("this database is already fully loaded");
                return;
            }

            //this.ReadVersion();

            this.InitializePackages();

            var scriptEntries = PackageStream.GetFileEntries(Path.Combine(RootPath, ScriptsPackageFile));


            var nationEntries =
                scriptEntries.Where(s => s.StartsWith(VehiclesFolderInPackage))
                    .Select(
                        s =>
                            s.Substring(VehiclesFolderInPackage.Length,
                                s.IndexOf("/", VehiclesFolderInPackage.Length, StringComparison.InvariantCulture) -
                                VehiclesFolderInPackage.Length))
                    .Distinct();
            
            foreach (var nationName in nationEntries)
            {
                if (Database.IsTechTreeFolder(nationName, scriptEntries))
                {
                    var nation = new NationalDatabase(this, nationName);
                    Log.InfoFormat("adding nation database: {0}", nation.Key);
                    this.Nations.Add(nation.Key, nation);
                }
            }

            this.LoadCommonData();

            this.LoadBenchmarkTanks();

            this.IsFullyLoaded = true;

        }

        private void InitializePackages()
        {
            this.PackageDatabase = new PackageDatabase(this);
        }

        private void LoadBenchmarkTanks()
        {
            Log.Info("loading benchmark tanks");

			this.BenchmarkTanks = new Dictionary<int, Dictionary<TankClass, BenchmarkTank>>();

			for (var tier = 1; tier <= MaxTankTier; ++tier)
            {
                this.BenchmarkTanks.Add(tier, new Dictionary<TankClass, BenchmarkTank>());
            }

            if (Cache.TryLoadBinary<BenchmarkTank[]>(this.Key, "benchmark_tanks", out BenchmarkTank[] benchmarkTanksArray))
            {
                foreach (var tank in benchmarkTanksArray)
                {
                    this.InitializeBenchmarkTank(tank);
                    this.BenchmarkTanks[tank.Tier].Add(tank.Class, tank);
                }
            }
            else
            {
                Log.Info("load benchmark tanks from cache failed, gonna regenerate");

                for (var tier = 1; tier <= MaxTankTier; ++tier)
                {
                    for (var tankClass = TankClass.LightTank; tankClass <= TankClass.SelfPropelledGun; ++tankClass)
                    {
                        var tanks = this.Tanks.Where(t => !t.IsObsoleted && t.Tier == tier && t.Class == tankClass);
                        if (tanks.Count() > 0)
                        {
                            var key = $"T{tier}{tankClass}BenchmarkTank";

                            Log.InfoFormat("generating benchmark tanks: {0}", key);

                            uint id = ((uint)tier << 4) + (uint)tankClass + 1;
                            var benchmark = BenchmarkTank.Create(key, id, tanks);
                            benchmark.Name = string.Format(App.GetLocalizedString("BenchmarkTankName"), App.GetLocalizedString(tankClass.ToString()), RomanNumberService.GetRomanNumber(tier));
                            benchmark.ShortName = string.Format(App.GetLocalizedString("BenchmarkTankShortName"), App.GetLocalizedString(tankClass.ToString() + "Short"), tier);
                            this.InitializeBenchmarkTank(benchmark);
                            this.BenchmarkTanks[tier].Add(tankClass, benchmark);
                        }
                    }
                }

                benchmarkTanksArray = this.BenchmarkTanks.Values.SelectMany(t => t.Values).ToArray();

                Log.Info("saveing benchmark tanks into cache");

                Cache.SaveBinary(this.Key, "benchmark_tanks", benchmarkTanksArray);
            }

            _benchmarkTankDict = benchmarkTanksArray.ToDictionary(t => t.Key);

        }

        private void InitializeBenchmarkTank(BenchmarkTank benchmark)
        {
            benchmark.AvailableConsumables = this.ConsumableDatabase.Consumables;
            benchmark.AvailableEquipments = this.EquipmentDatabase.Equipments;
            benchmark.Database = this;
        }

        private void LoadCommonData()
        {
            using(var stream = new PackageStream(this.PackageDatabase, CommonVehicleDataFile))
            using (var reader = new BigworldXmlReader(stream))
            {
                this.CommonVehicleData = new CommonVehicleDatabase(this);
                this.CommonVehicleData.Deserialize(reader);
            }

            using (var stream = new PackageStream(this.PackageDatabase, EquipmentDataFile))
            using (var reader = new BigworldXmlReader(stream))
            {
                this.EquipmentDatabase = new EquipmentDatabase(this);
                this.EquipmentDatabase.Deserialize(reader);
            }

            using (var stream = new PackageStream(this.PackageDatabase, ConsumableDataFile))
            using (var reader = new BigworldXmlReader(stream))
            {
                this.ConsumableDatabase = new ConsumableDatabase(this);
                this.ConsumableDatabase.Deserialize(reader);
            }

            using (var stream = new PackageStream(this.PackageDatabase, CrewSkillDataFile))
            using (var reader = new BigworldXmlReader(stream))
            {
                this.CrewDatabase = new CrewDatabase(this);
                this.CrewDatabase.Deserialize(reader);
            }
        }

        internal string ReadVersion()
        {
            using (var stream = File.OpenRead(Path.Combine(RootPath, VersionFile)))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    reader.ReadToDescendant("version");
                    this.Version = reader.ReadString().Trim();
                    this.Key = new DatabaseKey(this.RootPath, this.Version);
                    return this.Version;
                }
            }
        }

        private static bool IsTechTreeFolder(string nationName, string[] scriptEntries)
        {
            return scriptEntries.Contains($"{VehiclesFolderInPackage}{nationName}/list.xml")
                   && scriptEntries.Contains($"{VehiclesFolderInPackage}{nationName}/customization.xml");
        }

    }
}
