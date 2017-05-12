using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class Consumable : Accessory
    {
	    public string[] Tags { get; private set; }

	    public string[] IncompatibleTags { get; private set; }


	    public ConsumableScript Script { get; private set; }

	    public Consumable(Database database)
            : base(database)
        {

        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {

            switch (name)
            {
                case "tags":
                    this.Tags = reader.ReadString().Trim().Split(' ');
                    return true;
                case "incompatibleTags":
                    reader.ReadStartElement("installed");
                    this.IncompatibleTags = reader.ReadString().Trim().Split(' ');
                    reader.ReadEndElement();
                    return true;
                case "script":
                    this.Script = ConsumableScript.Resolve(reader, this.Database);
                    return true;
                case "id":
                    return false;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }


        public override double GetWeight(double tankWeight)
        {
            return 0;
        }
    }
}
