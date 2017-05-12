using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class StaticAdditiveDeviceScript : EquipmentScript
    {

        private string _attributeName;
        public string AttributeName => _attributeName;

	    private double _value = 0.0;
        public double Value => _value;

	    public override string[] EffectiveDomains => new[] {$"{"staticAdditiveDevice"}:{this.AttributeName}"};

	    public StaticAdditiveDeviceScript(Database database)
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
                case "value":
                    reader.Read(out _value);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }


        public override void Execute(ModificationContext sandbox, object args)
        {
            sandbox.SetValue(this.EffectiveDomains[0], _attributeName, _value);
        }
    }
}
