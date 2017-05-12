using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class DefaultArmorGroups : DatabaseObject, IDeserializable
    {
        private readonly Dictionary<string, ArmorGroup> _armorGroups;

        public DefaultArmorGroups(Database database)
            : base(database)
        {
            _armorGroups = new Dictionary<string, ArmorGroup>();
        }


        public void Deserialize(XmlReader reader)
        {
            while (reader.IsStartElement())
            {
                var key = reader.Name;
                reader.ReadStartElement();

                ArmorGroup armorGroup = new ArmorGroup();
                armorGroup.Deserialize(reader);

                _armorGroups.Add(key, armorGroup);

                reader.ReadEndElement();
            }
        }

        public ArmorGroup this[string key]
        {
            get
            {
				if (_armorGroups.TryGetValue(key, out ArmorGroup group))
					return group;

				return null;

            }
        }

        public bool TryGetValue(string key, out ArmorGroup armor)
        {
            return _armorGroups.TryGetValue(key, out armor);
        }

    }
}
