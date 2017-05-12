using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{

    [Serializable]
    internal abstract class DamageableModule : Module, IDamageable, IDamageableModule
    {
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

        private double _repairCostFactor;
        [Stat("RepairCostFactor", DataAnalysis.ComparisonMode.LowerBetter)]
        public double RepairCostFactor
        {
            get => _repairCostFactor;
	        protected set => _repairCostFactor = value;
        }

        private double _bulkHealthFactor;
        public double BulkHealthFactor
        {
            get => _bulkHealthFactor;
	        protected set => _bulkHealthFactor = value;
        }

        public DamageableModule(Database database)
            : base(database)
        {

        }

        protected override bool DeserializeSection(string name, XmlReader reader)
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
                    reader.Read(out _repairCostFactor);
                    return true;
                case "bulkHealthFactor":
                    reader.Read(out _bulkHealthFactor);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }

        }
    }
}
