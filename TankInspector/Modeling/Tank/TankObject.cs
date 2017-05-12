using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    [DebuggerDisplay("{Name}({Key})")]
    internal abstract class TankObject : Commodity, ICloneable, ITankObject, IHasModel
    {
        public abstract TankObjectType ObjectType { get; }

        public virtual TankObjectKey ObjectKey => new TankObjectKey(this.ObjectType, this.Key);

	    private string _name;

        [Stat("Name", DataAnalysis.ComparisonMode.NotComparable)]
        public string Name
        {
            get => _name;
	        protected set => _name = value;
        }

        private string _shortName;

        public string ShortName
        {
            get
            {
	            if (string.IsNullOrEmpty(_shortName))
                    return this.Name;
	            return _shortName;
            }
            protected set { _shortName = value; }
        }

        private int _tier;

        [Stat("Tier", DataAnalysis.ComparisonMode.Plain)]
        public int Tier
        {
            get => _tier;
	        protected set => _tier = value;
        }

        private ModelInfo _model;

        public ModelInfo Model
        {
            get => _model;
	        protected set => _model = value;
        }

        private List<UnlockInfo> _unlocks;
        public IEnumerable<UnlockInfo> Unlocks => _unlocks;

	    public TankObject(Database database)
            : base(database)
        {
            _unlocks = new List<UnlockInfo>();
            _model = new ModelInfo();
        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "userString":
                    reader.ReadLocalized(this.Database, out _name);
                    return true;
                case "shortUserString":
                    reader.ReadLocalized(this.Database, out _shortName);
                    return true;
                case "level":
                    // some levels are represented in float
                    double level;
                    reader.Read(out level);
                    _tier = (int)level;
                    return true;
                case "unlocks":
                    while (reader.IsStartElement())
                    {
                        var unlockInfo = new UnlockInfo();
                        unlockInfo.Deserialize(reader);
                        _unlocks.Add(unlockInfo);
                    }
                    return true;
                case "models":
                    _model.Deserialize(reader);
                    return true;
                case "hitTester":
                    string collisionModel;

                    if (reader.IsStartElement("collisionModelServer")) // post 9.15
                    {
                        reader.Skip();
                    }

                    if (reader.IsStartElement("collisionModelClient"))
                    {

                        reader.ReadStartElement("collisionModelClient");
                        reader.Read(out collisionModel);
                        reader.ReadEndElement();
                    }
                    else
                    {
                        reader.ReadStartElement("collisionModel");
                        reader.Read(out collisionModel);
                        reader.ReadEndElement();
                    }

					if (reader.IsStartElement("collisionModelServer")) // this could either be before or after the client model
					{
						reader.Skip();
					}

					_model.Collision = collisionModel;
                    return true;

                default:
                    return base.DeserializeSection(name, reader);
            }

        }

        public virtual object Clone()
        {
            var tankObject = (Module)this.MemberwiseClone();
            tankObject._unlocks = new List<UnlockInfo>();
            foreach (var unlockInfo in this._unlocks)
                tankObject._unlocks.Add(unlockInfo);

            return tankObject;
        }

        public override string ToString()
        {
            return $"{this.ObjectType.ToString()}: {this.Name}";
        }
    }
}
