using Smellyriver.Utilities;
using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class CrewDatabase : DatabaseObject, IDeserializable
    {

        public Dictionary<string, CrewSkill> CrewSkills { get; private set; }

        public Dictionary<CrewRoleType, List<CrewSkill>> RoleSkills { get; private set; }
        public Dictionary<CrewRoleType, CrewRole> CrewRoles { get; private set; }

        public CrewDatabase(Database database)
            : base(database)
        {
            
        }

        public void Deserialize(XmlReader reader)
        {
            CrewSkills = new Dictionary<string, CrewSkill>();
            RoleSkills = new Dictionary<CrewRoleType, List<CrewSkill>>();
            CrewRoles = new Dictionary<CrewRoleType, CrewRole>();

            reader.ReadStartElement();

            while(reader.IsStartElement())
            {
                if (reader.Name == "roles")
                {
                    reader.ReadStartElement();
                    while (reader.IsStartElement())
                    {
                        CrewRole role = new CrewRole(this.Database);
                        role.Deserialize(reader);
                        CrewRoles.Add(role.Type, role);
                    }
                    reader.ReadEndElement();
                }
                else if (reader.Name == "skills")
                {

                    reader.ReadStartElement();
                    while (reader.IsStartElement())
                    {
                        var key = reader.Name;
                        var skill = CrewSkill.Resolve(this.Database, reader);
                        CrewSkills.Add(key, skill);

                        CrewRoleType position;
                        if (key.StartsWith("commander_"))
                            position = CrewRoleType.Commander;
                        else if (key.StartsWith("driver_"))
                            position = CrewRoleType.Driver;
                        else if (key.StartsWith("gunner_"))
                            position = CrewRoleType.Gunner;
                        else if (key.StartsWith("loader_"))
                            position = CrewRoleType.Loader;
                        else if (key.StartsWith("radioman_"))
                            position = CrewRoleType.Radioman;
                        else
                            position = CrewRoleType.All;

                        var list = RoleSkills.GetOrCreate(position, () => new List<CrewSkill>());
                        list.Add(skill);
                    }

                    reader.ReadEndElement();
                }
                else
                    reader.Skip();
            }

            

        }
    }
}
