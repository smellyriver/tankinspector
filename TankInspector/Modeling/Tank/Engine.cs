using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Engine : DamageableModule,IEngine
    {
        public override ModuleType Type => ModuleType.Engine;

	    [Stat("FuelType", DataAnalysis.ComparisonMode.NotComparable)]
        public FuelType FuelType { get; private set; }

	    private double _horsePower;
        [Stat("EnginePowerFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double HorsePower => _horsePower;

	    private double _fireChance;
        [Stat("EngineFireChanceFullName", DataAnalysis.ComparisonMode.LowerBetter)]
        public double FireChance => _fireChance;

	    public Engine(Database database)
            : base(database)
        {

        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "tags":
                    this.FuelType = FuelTypeEx.Parse(reader.ReadString());
                    return true;
                case "power":
                    reader.Read(out _horsePower);
                    return true;
                case "fireStartingChance":
                    reader.Read(out _fireChance);
                    return true;
                case "sound":
                    return false;

                default:
                    if (base.DeserializeSection(name, reader))
                        return true;
                    else
                        return false;
            }
        }

    }
}
