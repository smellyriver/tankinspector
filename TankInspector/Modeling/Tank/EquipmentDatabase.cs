using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class EquipmentDatabase : DatabaseObject, IDeserializable
    {

        public Dictionary<string, Equipment> Equipments { get; private set; }

        public EquipmentDatabase(Database database)
            :base(database)
        {
            
        }
        public void Deserialize(XmlReader reader)
        {
            Equipments = new Dictionary<string, Equipment>();

            reader.ReadStartElement();
            while(reader.IsStartElement())
            {
                Equipment equipment = new Equipment(this.Database);
                equipment.Deserialize(reader);
                Equipments.Add(equipment.Key, equipment);
            }
        }
    }
}
