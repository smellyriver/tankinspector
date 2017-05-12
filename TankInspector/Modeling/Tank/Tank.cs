using Smellyriver.Collection;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.IO.XmlDecoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class Tank : TankObject, ILazy<Tank>, ITank
    {

        public override TankObjectType ObjectType => TankObjectType.Tank;

	    Tank ILazy<Tank>.Value
        {
            get
            {
                if (!_isLoaded)
                    this.Load();
                return this;
            }
        }

        protected override bool IsWrapped => false;

	    public string Description { get; }

	    public TankClass Class { get; }


	    private Camouflage _camouflage;
        public Camouflage Camouflage => _camouflage;

	    private Invisibility _invisibility;
        public Invisibility Invisibility => _invisibility;

	    public bool IsPremiumTank => this.CurrencyType == CurrencyType.Gold;

	    public IEnumerable<HierachyInfo<Tank>> Predecessors => this.Nation.TechTree.GetPredecessors<Tank>(this);

	    public IEnumerable<HierachyInfo<Tank>> Successors => this.Nation.TechTree.GetSuccessors<Tank>(this);

	    public bool IsObsoleted => !this.IsPremiumTank && !this.Predecessors.Any() && !this.Successors.Any();

	    [StatGroup("Crews")]
        public List<Crew> Crews { get; }

        [StatGroup("Chassis")]
        public Dictionary<string, Chassis> AvailableChassis { get; }
        Dictionary<string, IChassis> ITank.AvailableChassis
        {
            get { return this.AvailableChassis.ToDictionary(c => c.Key, c => (IChassis)c.Value); }
        }

        [StatGroup("Turrets")]
        public Dictionary<string, Turret> AvailableTurrets { get; }
        Dictionary<string, ITurret> ITank.AvailableTurrets
        {
            get { return this.AvailableTurrets.ToDictionary(t => t.Key, t => (ITurret)t.Value); }
        }

        [StatGroup("Engines")]
        public Dictionary<string, Engine> AvailableEngines { get; }
        Dictionary<string, IEngine> ITank.AvailableEngines
        {
            get { return this.AvailableEngines.ToDictionary(e => e.Key, e => (IEngine)e.Value); }
        }

        [StatGroup("Radios")]
        public Dictionary<string, Radio> AvailableRadios { get; }
        Dictionary<string, IRadio> ITank.AvailableRadios
        {
            get { return this.AvailableRadios.ToDictionary(r => r.Key, r => (IRadio)r.Value); }
        }

        public Dictionary<string, FuelTank> AvailableFuelTanks { get; }
        Dictionary<string, IFuelTank> ITank.AvailableFuelTanks
        {
            get { return this.AvailableFuelTanks.ToDictionary(f => f.Key, f => (IFuelTank)f.Value); }
        }

        [StatSubItem("FuelTank")]
        public FuelTank FuelTank => this.AvailableFuelTanks.Values.First();

	    private readonly Lazy<Dictionary<string, Equipment>> _lazyAvailableEquipments;
        public Dictionary<string, Equipment> AvailableEquipments => _lazyAvailableEquipments.Value;

	    private readonly Lazy<Dictionary<string, Consumable>> _lazyAvailableConsumables;
        public Dictionary<string, Consumable> AvailableConsumables => _lazyAvailableConsumables.Value;

	    [StatSubItem("Hull")]
        public Hull Hull { get; private set; }

	    IHull ITank.Hull => this.Hull;

	    // applied to query available modules, while iterating modules, use guns in turrets instead
        // BE AWARE: if a gun can be equipped in multiple turrets, only one instance of this gun is
        // listed here
        public Dictionary<string, Gun> AvailableGuns { get; }
        Dictionary<string, IGun> ITank.AvailableGuns
        {
            get { return this.AvailableGuns.ToDictionary(g => g.Key, g => (IGun)g.Value); }
        }

        [Stat("TankMatchmakingWeightFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double MatchMakingWeight
        {
            get
            {
                if (this.Nation.Database.CommonVehicleData.MatchmakingWeightRules == null)
                    return -1;

                return this.Nation.Database.CommonVehicleData.MatchmakingWeightRules.GetMatchmakingWeight(this);
            }
        }

        public IEnumerable<Module> AllModules
        {
            get
            {
                return ((IEnumerable<Module>)this.AvailableChassis.Values)
                    .Union((IEnumerable<Module>)this.AvailableTurrets.Values)
                    .Union((IEnumerable<Module>)this.AvailableTurrets.Values.SelectMany(turret => turret.AvailableGuns.Values))
                    .Union((IEnumerable<Module>)this.AvailableEngines.Values)
                    .Union((IEnumerable<Module>)this.AvailableRadios.Values)
                    .Union((IEnumerable<Module>)this.AvailableFuelTanks.Values)
                    .Union(new Module[] { this.Hull });
            }
        }

        private double _repairCostFactor;
        [Stat("RepairCostFactor", DataAnalysis.ComparisonMode.LowerBetter)]
        public double RepairCostFactor => _repairCostFactor;

	    private double _crewExperienceFactor;
        public double CrewExperienceFactor => _crewExperienceFactor;

	    private SpeedLimits _speedLimits;
        [ExpandableStat]
        public SpeedLimits SpeedLimits => _speedLimits;

	    private readonly Dictionary<Type, object> _availableModuleDictionaries;

        public NationalDatabase Nation { get; }

        public string NationKey => this.Nation.Key;

	    public uint NationId
        {
            get
            {
                switch (this.NationKey)
                {
                    case "ussr":
                        return 0;
                    case "germany":
                        return 1;
                    case "usa":
                        return 2;
                    case "china":
                        return 3;
                    case "france":
                        return 4;
                    case "uk":
                        return 5;
                    case "japan":
                        return 6;
                    default:
                        return 7;
                }
            }
        }

        /// <summary>
        /// the unique tank ID within its nation
        /// </summary>
        public uint Id { get; }

        public uint TypeId => 1;

	    public BattleTierSpan BattleTiers { get; internal set; }

        public CamouflageValue CamouflageValue { get; internal set; }

        public string HyphenFullKey => $"{this.Nation.Key}-{this.Key}";

	    public string ColonFullKey => $"{this.Nation.Key}:{this.Key}";

	    public string IconKey => this.HyphenFullKey;

	    // todo: this is kludge, find a better way
        public bool IsValid => TankIcon.Exists(this.Database.RootPath, this.HyphenFullKey);

	    public string[] Tags { get; }

        
        private bool _isLoaded;

        public Tank(TankInfo info)
            : base(info.Database)
        {
            this.Name = info.Name;
            this.Tier = info.Tier;
            this.Description = info.Description;
            this.Class = info.Class;
            this.Key = info.Key;
            this.Nation = info.Nation;
            this.Id = info.Id;
            this.Price = info.Price;
            this.ShortName = info.ShortName;
            this.NotInShop = info.NotInShop;
            this.CurrencyType = info.CurrencyType;
            this.Tags = info.Tags;

            this.Crews = new List<Crew>();

            _availableModuleDictionaries = new Dictionary<Type, object>();

            this.AvailableChassis = new Dictionary<string, Chassis>();
            _availableModuleDictionaries.Add(typeof(Chassis), AvailableChassis);

            this.AvailableEngines = new Dictionary<string, Engine>();
            _availableModuleDictionaries.Add(typeof(Engine), AvailableEngines);

            this.AvailableRadios = new Dictionary<string, Radio>();
            _availableModuleDictionaries.Add(typeof(Radio), AvailableRadios);

            this.AvailableTurrets = new Dictionary<string, Turret>();
            _availableModuleDictionaries.Add(typeof(Turret), AvailableTurrets);

            this.AvailableFuelTanks = new Dictionary<string, FuelTank>();
            _availableModuleDictionaries.Add(typeof(FuelTank), AvailableFuelTanks);

            this.AvailableGuns = new Dictionary<string, Gun>();
            _availableModuleDictionaries.Add(typeof(Gun), AvailableGuns);

            _lazyAvailableEquipments = new Lazy<Dictionary<string, Equipment>>(GetAvailableEquipments);
            _lazyAvailableConsumables = new Lazy<Dictionary<string, Consumable>>(GetAvailableConsumables);
        }

        private Dictionary<string, Consumable> GetAvailableConsumables()
        {
            return this.Nation.Database.ConsumableDatabase.Consumables
                .Where(p => p.Value.CanBeUsedBy(this))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private Dictionary<string, Equipment> GetAvailableEquipments()
        {
            return this.Nation.Database.EquipmentDatabase.Equipments
                .Where(p => p.Value.CanBeUsedBy(this))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private void Load()
        {
            using (
                var stream = new PackageStream(this.Database.PackageDatabase,
                    this.Nation.GetPackageFileName(this.Key + ".xml")))
            {
                using (var reader = new BigworldXmlReader(stream))
                {
                    reader.ReadStartElement();
                    this.Deserialize(reader);
                    reader.ReadEndElement();
                }
            }

            this.BattleTiers = BattleTierRules.Current.GetBattleTiers(this);
            this.CamouflageValue = CamouflageValues.Current.GetCamouflageValue(this);

            _isLoaded = true;
        }

        public TObject GetTankObject<TObject>(string key)
        {
            IDictionary<string, TObject> storage;
			TObject result;
			if (_availableModuleDictionaries.TryGetValue(typeof(TObject), out object storageObject))
            {
                storage = (Dictionary<string, TObject>)storageObject;
                if (storage.TryGetValue(key, out result))
                    return result;
            }

            storage = this.Nation.GetSharedObjects<TObject>();
            if (storage.TryGetValue(key, out result))
                return result;

            throw new KeyNotFoundException();
        }

        private void SyncSpeedLimitsWithChassis()
        {
            foreach (var chassis in this.AvailableChassis.Values)
                chassis.SpeedLimits = this.SpeedLimits;
        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case SectionDeserializableImpl.TitleToken:
                    return false;
                case "crew":
                    while (reader.IsStartElement())
                    {
                        var crew = new Crew();
                        crew.Deserialize(reader);
                        this.Crews.Add(crew);
                    }
                    return true;
                case "speedLimits":
                    reader.Read(out _speedLimits);
                    this.SyncSpeedLimitsWithChassis();
                    return true;

                case "repairCost":
                    reader.Read(out _repairCostFactor);
                    return true;

                case "crewXpFactor":
                    reader.Read(out _crewExperienceFactor);
                    return true;

                case "hull":
                    this.Hull = new Hull(this.Database);
                    this.Hull.Owner = this;
                    this.Hull.Deserialize(reader);
                    return true;

                case "chassis":
                    this.DeserializeModules(reader, AvailableChassis, () => new Chassis(this.Database));
                    this.SyncSpeedLimitsWithChassis();
                    return true;

                case "turrets0":
                    this.DeserializeModules(reader, AvailableTurrets, () => new Turret(this.Database));
                    foreach (var gun in this.AvailableTurrets.Values.SelectMany(turret => turret.AvailableGuns.Values))
                    {
                        if (!this.AvailableGuns.ContainsKey(gun.Key))
                            this.AvailableGuns.Add(gun.Key, gun);
                    }
                    return true;

                case "engines":
                    this.DeserializeModules(reader, AvailableEngines, () => new Engine(this.Database));
                    return true;

                case "fuelTanks":
                    this.DeserializeModules(reader, AvailableFuelTanks, () => new FuelTank(this.Database));
                    return true;

                case "radios":
                    this.DeserializeModules(reader, AvailableRadios, () => new Radio(this.Database));
                    return true;

                case "camouflage":
                    reader.Read(out _camouflage);
                    return true;

                case "invisibility":
                    reader.Read(out _invisibility);
                    return true;

                case "effects":
                case "emblems":
                case "horns":
                case "extras":
                case "customizationNation":
                case "premiumVehicleXPFactor":
                case "repaintParameters":
                case "clientAdjustmentFactors":
                case "defaultCamouflageIDs":
                case "customDefaultCamouflage":
                case "physics":
                case "siege_mode":
                case "hull_aiming":
                case "isRotationStill":
                case "useHullZ":
                    return false;

                default:
                    if (base.DeserializeSection(name, reader))
                        return true;
                    else
                        return false;
            }

        }



        internal TModule DeserializeModules<TModule>(XmlReader reader, Dictionary<string, TModule> storage, Func<TModule> factory, TankObject owner = null)
            where TModule : Module, ICloneable
        {
            var sharedModules = this.Nation.GetSharedObjects<TModule>();

            TModule firstModule = null;

            while (reader.IsStartElement())
            {
				if (sharedModules.TryGetValue(reader.Name, out TModule module))
					module = (TModule)module.Clone();
				else
					module = factory();

				module.Owner = owner == null ? this : owner;

                module.Deserialize(reader);

                if (firstModule == null)
                    firstModule = module;

                storage.Add(module.Key, module);
                this.Nation.RegisterModule(module);

            }

            return firstModule;
        }


        public override string ToString()
        {
            return $"Tank: {this.Name}";
        }
    }
}
