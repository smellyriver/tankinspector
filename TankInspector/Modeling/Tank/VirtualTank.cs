using Smellyriver.TankInspector.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class VirtualTank : VirtualTankObject, ITank
    {


        public override TankObjectType ObjectType => TankObjectType.Tank;

	    [NonSerialized]
        private Dictionary<string, IChassis> _availableChassis;
        Dictionary<string, IChassis> ITank.AvailableChassis => _availableChassis;

	    [NonSerialized]
        private Dictionary<string, IEngine> _availableEngines;
        Dictionary<string, IEngine> ITank.AvailableEngines => _availableEngines;

	    [NonSerialized]
        private Dictionary<string, IFuelTank> _availableFuelTanks;
        Dictionary<string, IFuelTank> ITank.AvailableFuelTanks => _availableFuelTanks;

	    [NonSerialized]
        private Dictionary<string, IGun> _availableGuns;
        Dictionary<string, IGun> ITank.AvailableGuns => _availableGuns;

	    [NonSerialized]
        private Dictionary<string, IRadio> _availableRadios;
        Dictionary<string, IRadio> ITank.AvailableRadios => _availableRadios;

	    [NonSerialized]
        private Dictionary<string, ITurret> _availableTurrets;
        Dictionary<string, ITurret> ITank.AvailableTurrets => _availableTurrets;


	    public virtual string IconKey => TankIcon.VirtualTankIconKey;

	    public virtual string ColonFullKey => $"virtual:{this.Key}";

	    public string NationKey { get; internal set; }


        public uint Id { get; internal set; }

        public abstract uint TypeId { get; }

        public uint NationId { get; internal set; }

        [NonSerialized]
        private Database _database;
        public Database Database
        {
            get => _database;
	        internal set => _database = value;
        }

        private IChassis _chassis;
        public IChassis Chassis
        {
            get => _chassis;
	        internal set
            {
                _chassis = value;
                _availableChassis.Clear();
                _availableChassis.Add(_chassis.Key, _chassis);
            }
        }

        private IEngine _engine;
        public IEngine Engine
        {
            get => _engine;
	        internal set
            {
                _engine = value;
                _availableEngines.Clear();
                _availableEngines.Add(_engine.Key, _engine);
            }
        }

        private IFuelTank _fuelTank;
        public IFuelTank FuelTank
        {
            get => _fuelTank;
	        internal set
            {
                _fuelTank = value;
                _availableFuelTanks.Clear();
                _availableFuelTanks.Add(_fuelTank.Key, _fuelTank);
            }
        }

        [NonSerialized]
        private IGun _gun;
        public IGun Gun
        {
            get => _gun;
	        internal set
            {
                _gun = value;
                _availableGuns.Clear();
                _availableGuns.Add(_gun.Key, _gun);
            }
        }

        private IRadio _radio;
        public IRadio Radio
        {
            get => _radio;
	        internal set
            {
                _radio = value;
                _availableRadios.Clear();
                _availableRadios.Add(_radio.Key, _radio);
            }
        }

        private ITurret _turret;
        public ITurret Turret
        {
            get => _turret;
	        internal set
            {
                _turret = value;
                _availableTurrets.Clear();
                _availableTurrets.Add(_turret.Key, _turret);
            }
        }

        public BattleTierSpan BattleTiers { get; internal set; }

        public CamouflageValue CamouflageValue { get; internal set; }
        public Invisibility Invisibility { get; internal set; }

        public TankClass Class { get; internal set; }

        public double CrewExperienceFactor { get; internal set; }

        public List<Crew> Crews { get; internal set; }

        public string Description { get; internal set; }

        public IHull Hull { get; internal set; }

        public double MatchMakingWeight { get; internal set; }

        public double RepairCostFactor { get; internal set; }

        public SpeedLimits SpeedLimits => _chassis.SpeedLimits;

	    [NonSerialized]
        private Dictionary<string, Equipment> _availableEquipments;
        public Dictionary<string, Equipment> AvailableEquipments
        {
            get => _availableEquipments;
	        internal set => _availableEquipments = value;
        }

        [NonSerialized]
        private Dictionary<string, Consumable> _availableConsumables;
        public Dictionary<string, Consumable> AvailableConsumables
        {
            get => _availableConsumables;
	        internal set => _availableConsumables = value;
        }

        public string[] Tags { get; internal set; }

        public VirtualTank()
        {
            _availableChassis = new Dictionary<string, IChassis>();
            _availableEngines = new Dictionary<string, IEngine>();
            _availableFuelTanks = new Dictionary<string, IFuelTank>();
            _availableGuns = new Dictionary<string, IGun>();
            _availableRadios = new Dictionary<string, IRadio>();
            _availableTurrets = new Dictionary<string, ITurret>();
            this.Tags = new string[0];

            this.Crews = new List<Crew>();
        }

        protected override void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);

            _availableChassis = new Dictionary<string, IChassis>();
            _availableChassis.Add(_chassis.Key, _chassis);

            _availableEngines = new Dictionary<string, IEngine>();
            _availableEngines.Add(_engine.Key, _engine);

            _availableFuelTanks = new Dictionary<string, IFuelTank>();
            _availableFuelTanks.Add(_fuelTank.Key, _fuelTank);

            _availableRadios = new Dictionary<string, IRadio>();
            _availableRadios.Add(_radio.Key, _radio);

            _availableTurrets = new Dictionary<string, ITurret>();
            _availableTurrets.Add(_turret.Key, _turret);

            // we should retrieve the gun by querying _turret.AvailableGuns, unfortunately 
            // in a deserialization sequence, _turret.AvailableGuns is initialized in its
            // OnDeserialized method, which is not guaranteed to be called after this method
            // (binary serialization sucks here)

            _gun = ((VirtualTurret)_turret).Gun;

            _availableGuns = new Dictionary<string, IGun>();
            _availableGuns.Add(_gun.Key, _gun);
        }





    }
}
