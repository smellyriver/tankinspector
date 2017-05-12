using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class Equipment : Accessory
    {
        

        private bool _removable;
        public bool Removable => _removable;

	    public EquipmentScript Script { get; private set; }

	    public Equipment(Database database)
            : base(database)
        {

        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {

            switch (name)
            {
                case "removable":
                    reader.Read(out _removable);
                    return true;
                case "script":
                    this.Script = EquipmentScript.Resolve(reader, this.Database);
                    return true;
                case "id":
                    return false;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }

        public override double GetWeight(double tankWeight)
        {
            return this.Script.GetWeight(tankWeight);
        }
    }
}
