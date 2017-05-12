using Smellyriver.TankInspector.IO.XmlDecoding;
using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.Collection;
using Smellyriver.TankInspector.IO;
using log4net;
using System.Reflection;

namespace Smellyriver.TankInspector.Modeling
{
	internal class NationalDatabase : DatabaseObject, ILazy<NationalDatabase>
	{
		private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

		NationalDatabase ILazy<NationalDatabase>.Value
		{
			get
			{
				if (!this._isLoaded)
					this.Load();

				return this;
			}
		}

		public string Key { get; }

		public string Name => this.Database.TextData.GetText($"#nations:{this.Key}");

		private const string ShellXmlFile = @"components/shells.xml";
		private const string RadioXmlFile = @"components/radios.xml";
		private const string FuelTanksXmlFile = @"components/fueltanks.xml";
		private const string ChassisXmlFile = @"components/chassis.xml";
		private const string EnginesXmlFile = @"components/engines.xml";
		private const string GunsXmlFile = @"components/guns.xml";
		private const string TurretsXmlFile = @"components/turrets.xml";

		public Dictionary<string, Shell> Shells { get; private set; }
		public Dictionary<string, Radio> SharedRadios { get; }
		public Dictionary<string, FuelTank> SharedFuelTanks { get; }
		public Dictionary<string, Chassis> SharedChassis { get; }
		public Dictionary<string, Engine> SharedEngines { get; }
		public Dictionary<string, Gun> SharedGuns { get; }
		public Dictionary<string, Turret> SharedTurrets { get; }
		public LazyDictionary<string, Tank> Tanks { get; }

		public List<TankInfo> TankInfoCollection { get; private set; }

		private readonly Dictionary<Type, object> _sharedModuleDictionaries;

		private readonly Dictionary<TankObjectKey, Module> _allLoadedModules;

		private bool _isLoaded;

		public TechTree TechTree { get; private set; }

		internal NationalDatabase(Database database, string key)
			: base(database)
		{

			Log.InfoFormat("initializing NationalDatabase: key='{0}'", key);

			this.Key = key;

			_sharedModuleDictionaries = new Dictionary<Type, object>();

			this.Shells = new Dictionary<string, Shell>();
			_sharedModuleDictionaries.Add(typeof(Shell), this.Shells);

			this.SharedRadios = new Dictionary<string, Radio>();
			_sharedModuleDictionaries.Add(typeof(Radio), this.SharedRadios);

			this.SharedFuelTanks = new Dictionary<string, FuelTank>();
			_sharedModuleDictionaries.Add(typeof(FuelTank), this.SharedFuelTanks);

			this.SharedChassis = new Dictionary<string, Chassis>();
			_sharedModuleDictionaries.Add(typeof(Chassis), this.SharedChassis);

			this.SharedEngines = new Dictionary<string, Engine>();
			_sharedModuleDictionaries.Add(typeof(Engine), this.SharedEngines);

			this.SharedGuns = new Dictionary<string, Gun>();
			_sharedModuleDictionaries.Add(typeof(Gun), this.SharedGuns);

			this.SharedTurrets = new Dictionary<string, Turret>();
			_sharedModuleDictionaries.Add(typeof(Turret), this.SharedTurrets);

			this.Tanks = new LazyDictionary<string, Tank>();
			_sharedModuleDictionaries.Add(typeof(Tank), this.Tanks);

			_allLoadedModules = new Dictionary<TankObjectKey, Module>();

			this.LoadTankInfo();
		}

		public string GetPackageFileName(string path)
		{
			return $"{Database.VehiclesFolderInPackage}{this.Key}/{path}";
		}

		private void LoadTankInfo()
		{

			Log.Info("loading tank info");

			this.TankInfoCollection = new List<TankInfo>();

			using (var stream = new PackageStream(this.Database.PackageDatabase, this.GetPackageFileName("list.xml")))
			{
				using (var reader = new BigworldXmlReader(stream))
				{
					reader.ReadStartElement("FromStream");

					while (reader.IsStartElement())
					{
						var tankInfo = new TankInfo(this);
						tankInfo.Deserialize(reader);
						this.TankInfoCollection.Add(tankInfo);
					}

					reader.ReadEndElement();
				}
			}
		}


		private void Load()
		{
			this.LoadSharedItems();
			this.LoadTanks();

			this.ResolveTechTree();

			this._isLoaded = true;
		}

		private void LoadSharedItems()
		{
			if (Cache.TryLoadBinary(this.Database.Key, this.Key + "_shells", out Shell[] shells))
				this.Shells = shells.ToDictionary(shell => shell.Key);
			else
			{
				this.LoadShells(ShellXmlFile);
				Cache.SaveBinary(this.Database.Key, this.Key + "_shells", this.Shells.Values.ToArray());
			}

			Log.Info("reading shared radio data");
			this.LoadSharedModules(RadioXmlFile, "radios", this.SharedRadios, () => new Radio(this.Database));
			Log.Info("reading shared fuel tank data");
			this.LoadSharedModules(FuelTanksXmlFile, "fueltanks", this.SharedFuelTanks, () => new FuelTank(this.Database));
			Log.Info("reading shared chassis data");
			this.LoadSharedModules(ChassisXmlFile, "chassis", this.SharedChassis, () => new Chassis(this.Database));
			Log.Info("reading shared engine data");
			this.LoadSharedModules(EnginesXmlFile, "engines", this.SharedEngines, () => new Engine(this.Database));
			Log.Info("reading shared gun data");
			this.LoadSharedModules(GunsXmlFile, "guns", this.SharedGuns, () => new Gun(this.Database));
			Log.Info("reading shared turret data");
			this.LoadSharedModules(TurretsXmlFile, "turrets", this.SharedTurrets, () => new Turret(this.Database));
		}


		private void ResolveTechTree()
		{
			if (Cache.TryLoadBinary(this.Database.Key, this.Key + "_techtree", out TechTree techtree))
			{
				techtree.Nation = this;
				this.TechTree = techtree;
			}
			else
			{
				Log.Info("load tech tree from cache failed, gonna regenerate");
				this.TechTree = new TechTree(this);

				Log.Info("saveing tech tree into cache");
				Cache.SaveBinary(this.Database.Key, this.Key + "_techtree", this.TechTree);
			}
		}

		public IDictionary<string, TObject> GetSharedObjects<TObject>()
		{
			return (IDictionary<string, TObject>)_sharedModuleDictionaries[typeof(TObject)];
		}

		private void LoadTanks()
		{
			foreach (var tankInfo in this.TankInfoCollection)
				this.Tanks.Add(tankInfo.Key, new Tank(tankInfo));
		}


		private void LoadSharedModules<TModule>(string file, string moduleName, Dictionary<string, TModule> storage, Func<TModule> factory, bool useCache = true)
			where TModule : Module
		{

			var cacheName = $"{this.Key}_{moduleName}";
			if (useCache && Cache.TryLoadBinary(this.Database.Key, cacheName, out TModule[] modules))
			{
				foreach (var module in modules)
				{
					module.Nation = this;
					storage.Add(module.Key, module);
					this.RegisterModule(module);
				}
			}
			else
			{
				Log.Info("load shared module data from cache failed, gonna regenerate");



				using (var stream = new PackageStream(this.Database.PackageDatabase, this.GetPackageFileName(file)))
				{
					using (var reader = new BigworldXmlReader(stream))
					{

						reader.ReadStartElement();
						while (reader.IsStartElement())
						{
							if (reader.Name == "shared")
							{
								reader.ReadStartElement();
								while (reader.IsStartElement())
								{
									var module = factory();
									module.Nation = this;
									module.Deserialize(reader);
									storage.Add(module.Key, module);
									this.RegisterModule(module);
								}
								reader.ReadEndElement();
							}
							else
							{
								reader.Skip();
							}

						}

						reader.ReadEndElement();
					}
				}

				if (useCache)
				{
					Log.Info("saving shared module data into cache");
					Cache.SaveBinary(this.Database.Key, cacheName, storage.Values.ToArray());
				}
			}
		}

		internal void RegisterModule(Module module)
		{
			_allLoadedModules.Add(module.ObjectKey, module);
		}

		internal void RegisterModules(IEnumerable<Module> modules)
		{
			foreach (var module in modules)
				this.RegisterModule(module);
		}

		private void LoadShells(string file)
		{
			Log.Info("loading shell data");
			using (var stream = new PackageStream(this.Database.PackageDatabase, this.GetPackageFileName(file)))
			{
				using (var reader = new BigworldXmlReader(stream))
				{

					reader.ReadStartElement("FromStream");
					while (reader.IsStartElement())
					{
						if (reader.Name == "icons")
						{
							reader.Skip();
						}
						else
						{
							var shell = new Shell(this.Database);
							shell.Deserialize(reader);
							this.Shells.Add(shell.Key, shell);
						}

					}

					reader.ReadEndElement();
				}
			}

		}

		public TankObject GetTankObject(TankObjectKey key)
		{
			if (key.Type == TankObjectType.Tank)
				return this.Tanks[key.Key];
			return _allLoadedModules[key];
		}
	}
}
