using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class StaticFactorDeviceScript : EquipmentScript
    {

        private string _attributeName;
        public string AttributeName => _attributeName;

	    private double _factor = 1.0;
        public double Factor => _factor;

	    public override string[] EffectiveDomains => new[] {$"{"staticFactorDevice"}:{this.AttributeName}"};

	    public StaticFactorDeviceScript(Database database)
            : base(database)
        {

        }

        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "attribute":
                    reader.Read(out _attributeName);
                    return true;
                case "factor":
                    reader.Read(out _factor);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }


        public override void Execute(ModificationContext sandbox, object args)
        {
            sandbox.SetValue(this.EffectiveDomains[0], _attributeName, _factor);
        }
    }
}
