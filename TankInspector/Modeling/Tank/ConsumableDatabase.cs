using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class ConsumableDatabase : DatabaseObject, IDeserializable
    {


        public Dictionary<string, Consumable> Consumables { get; private set; }

        public ConsumableDatabase(Database database)
            : base(database)
        {

        }

        public void Deserialize(XmlReader reader)
        {
            Consumables = new Dictionary<string, Consumable>();

            reader.ReadStartElement();
            while (reader.IsStartElement())
            {
                Consumable consumable = new Consumable(this.Database);
                consumable.Deserialize(reader);
                if (consumable.Key != "xmlref")
                {
                    Consumables.Add(consumable.Key, consumable);
                }
            }
        }
    }
}
