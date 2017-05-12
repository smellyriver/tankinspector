using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class DamageableComponent : DatabaseObject, IDamageable, ISectionDeserializable, IDamageableComponent
    {
        public abstract string Name { get; }

        private double _maxHealth;
        [Stat("Health", DataAnalysis.ComparisonMode.HigherBetter)]
        public double MaxHealth
        {
            get => _maxHealth;
	        protected set => _maxHealth = value;
        }

        private double _maxRegenHealth;
        [Stat("RegeneratedHealth", DataAnalysis.ComparisonMode.Plain)]
        public double MaxRegenHealth
        {
            get => _maxRegenHealth;
	        protected set => _maxRegenHealth = value;
        }

        private double _repairCost;
        [Stat("RepairCostFactor", DataAnalysis.ComparisonMode.LowerBetter)]
        public double RepairCostFactor
        {
            get => _repairCost;
	        protected set => _repairCost = value;
        }

        private double _bulkHealthFactor;
        public double BulkHealthFactor
        {
            get => _bulkHealthFactor;
	        protected set => _bulkHealthFactor = value;
        }

        public DamageableComponent(Database database)
            : base(database)
        {

        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            return this.DeserializeSection(name, reader);
        }

        protected virtual bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "maxHealth":
                    reader.Read(out _maxHealth);
                    return true;
                case "maxRegenHealth":
                    reader.Read(out _maxRegenHealth);
                    return true;
                case "repairCost":
                    reader.Read(out _repairCost);
                    return true;
                case "bulkHealthFactor":
                    reader.Read(out _bulkHealthFactor);
                    return true;
                default:
                    return false;
            }

        }

        bool ISectionDeserializable.IsWrapped => false;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
