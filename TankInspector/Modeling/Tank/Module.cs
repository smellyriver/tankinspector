using System;
using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class Module : TankObject, IModule
    {

        public override TankObjectType ObjectType => (TankObjectType)this.Type;

	    public override TankObjectKey ObjectKey => TankObjectKey.Create(this.Owner == null ? (TankObjectKey?)null : this.Owner.ObjectKey, this.ObjectType, this.Key);

	    public abstract ModuleType Type { get; }

        [NonSerialized]
        private TankObject _owner;
        internal TankObject Owner
        {
            get => _owner;
	        set => _owner = value;
        }


        private double _weight;

        [Stat("Weight", DataAnalysis.ComparisonMode.Plain)]
        public double Weight
        {
            get => _weight;
	        protected set => _weight = value;
        }

        [NonSerialized]
        private NationalDatabase _nation;
        // todo: use deserialization
        internal NationalDatabase Nation
        {
            get => _nation;
	        set => _nation = value;
        }

        public IEnumerable<HierachyInfo<TankObject>> Predecessors => this.Nation.TechTree.GetPredecessors<TankObject>(this);


	    public IEnumerable<HierachyInfo<TankObject>> Successors => this.Nation.TechTree.GetSuccessors<TankObject>(this);

	    public Module(Database database)
            : base(database)
        {

        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case null:
                    reader.ReadString();    // shared
                    return true;

                case "weight":
                    reader.Read(out _weight);
                    return true;

                default:
                    return base.DeserializeSection(name, reader);
            }

        }

        public override object Clone()
        {
            var result = (Module)base.Clone();
            result.Model = (ModelInfo)this.Model.Clone();
            return result;
        }

    }
}
