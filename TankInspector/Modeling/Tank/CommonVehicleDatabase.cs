using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class CommonVehicleDatabase : DatabaseObject, ISectionDeserializable
    {

        public MatchmakingWeightRules MatchmakingWeightRules { get; private set; }
        public DefaultArmorGroups DefaultArmorGroups { get; private set; }

        public CommonVehicleDatabase(Database database)
            : base(database)
        {
            
        }

        public bool IsWrapped => true;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        public bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "balance":
                    this.MatchmakingWeightRules = new MatchmakingWeightRules();
                    this.MatchmakingWeightRules.Deserialize(reader);
                    return true;
                    
                case "materials":
                    this.DefaultArmorGroups = new DefaultArmorGroups(this.Database);
                    this.DefaultArmorGroups.Deserialize(reader);
                    return true;

                case "lodLevels":
                case "miscParams":
                case "defaultVehicleEffects":
                case "extras":
                case "deviceExtras":
                    return false;


                default:
                    return false;
            }
        }

    }
}
