using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class CrewRole : DatabaseObject, ISectionDeserializable
    {

        public CrewRoleType Type { get; private set; }

        public string Key { get; private set; }

        private string _name;
        public string Name => _name;

	    private string _icon;
        public string Icon => _icon;

	    public CrewRole(Database database)
            : base(database)
        {
                
        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case SectionDeserializableImpl.TitleToken:
                    this.Key = reader.Name;
                    this.Type = CrewRoleTypeEx.Parse(this.Key);
                    return true;
                case "userString":
                    reader.ReadLocalized(this.Database, out _name);
                    return true;
                case "icon":
                    reader.Read(out _icon);
                    return true;
                case "description":
                    return false;
                default:
                    return false;
            }

        }


        bool ISectionDeserializable.IsWrapped => true;
    }
}
