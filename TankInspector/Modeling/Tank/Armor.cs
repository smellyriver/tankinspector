using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Armor : DatabaseObject, IDeserializable
    {
        [NonSerialized]
        private readonly Dictionary<string, ArmorGroup> _armorGroups;
        public Dictionary<string, ArmorGroup> ArmorGroups => _armorGroups;

	    private string[] _keys;
        private ArmorGroup[] _values;

        public Armor(Database database)
            : base(database)
        {
            _armorGroups = new Dictionary<string, ArmorGroup>();
        }

        public bool TryGetArmorValue(string key, out ArmorGroup value)
        {
	        if (_armorGroups.TryGetValue(key, out value))
                return true;
	        return this.Database.CommonVehicleData.DefaultArmorGroups.TryGetValue(key, out value);
        }

        public double GetArmorValue(string key)
        {
	        if (key != null && this.ArmorGroups.TryGetValue(key, out ArmorGroup group))
				return group.Value;
	        return 0.0;
        }

        public void Deserialize(XmlReader reader)
        {
            while (reader.IsStartElement())
            {
                var key = reader.Name;
                reader.ReadStartElement();


				if (!_armorGroups.TryGetValue(key, out ArmorGroup armorGroup))
				{
					armorGroup = this.Database.CommonVehicleData.DefaultArmorGroups[key];
					if (armorGroup == null)
						armorGroup = new ArmorGroup();
					else
						armorGroup = armorGroup.Clone();

					_armorGroups.Add(key, armorGroup);
				}

				armorGroup.Deserialize(reader);

                reader.ReadEndElement();
            }
        }

        

        protected override void OnSerializing(StreamingContext context)
        {
            base.OnSerializing(context);
            _keys = _armorGroups.Keys.ToArray();
            _values = _armorGroups.Values.ToArray();
        }

        protected override void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);
            for (int i = 0; i < _keys.Length; ++i)
                _armorGroups.Add(_keys[i], _values[i]);
        }
    }
}
