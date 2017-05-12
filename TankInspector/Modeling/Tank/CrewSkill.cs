using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal abstract class CrewSkill : CrewSkillBase, ISectionDeserializable, IDatabaseObject
    {

        public static CrewSkill Resolve(Database database, XmlReader reader)
        {
            var key = reader.Name;
            CrewSkill skill;
            switch (key)
            {
                case "repair":
                    skill = new RepairSkill(database);
                    break;
                case "fireFighting":
                    skill = new FireFightingSkill(database);
                    break;
                case "camouflage":
                    skill = new CamouflageSkill(database);
                    break;
                case "brotherhood":
                    skill = new BrotherhoodSkill(database);
                    break;
                case "commander_tutor":
                    skill = new TutorSkill(database);
                    break;
                case "commander_expert":
                    skill = new ExpertSkill(database);
                    break;
                case "commander_universalist":
                    skill = new UniversalistSkill(database);
                    break;
                case "commander_sixthSense":
                    skill = new SixthSenseSkill(database);
                    break;
                case "commander_eagleEye":
                    skill = new EagleEyeSkill(database);
                    break;
                case "driver_tidyPerson":
                    skill = new TidyPersonSkill(database);
                    break;
                case "driver_smoothDriving":
                    skill = new SmoothDrivingSkill(database);
                    break;
                case "driver_virtuoso":
                    skill = new VirtuosoSkill(database);
                    break;
                case "driver_badRoadsKing":
                    skill = new BadRoadsKingSkill(database);
                    break;
                case "driver_rammingMaster":
                    skill = new RammingMasterSkill(database);
                    break;
                case "gunner_smoothTurret":
                    skill = new SmoothTurretSkill(database);
                    break;
                case "gunner_gunsmith":
                    skill = new GunsmithSkill(database);
                    break;
                case "gunner_sniper":
                    skill = new SniperSkill(database);
                    break;
                case "gunner_rancorous":
                    skill = new RancorousSkill(database);
                    break;
                case "gunner_woodHunter":
                    skill = new WoodHunterSkill(database);
                    break;
                case "loader_pedant":
                    skill = new PedantSkill(database);
                    break;
                case "loader_desperado":
                    skill = new DesperadoSkill(database);
                    break;
                case "loader_intuition":
                    skill = new IntuitionSkill(database);
                    break;
                case "radioman_finder":
                    skill = new FinderSkill(database);
                    break;
                case "radioman_inventor":
                    skill = new InventorSkill(database);
                    break;
                case "radioman_lastEffort":
                    skill = new LastEffortSkill(database);
                    break;
                case "radioman_retransmitter":
                    skill = new RetransmitterSkill(database);
                    break;
                default:
                    throw new NotSupportedException();
            }

            skill.Deserialize(reader);

            return skill;
        }

        private string _name;
        public override string Name => _name;

	    private string _shortDescription;
        public override string ShortDescription => _shortDescription;

	    private string _description;
        public override string Description => _description;

	    private string _icon;
        public override string Icon => _icon;

	    public Database Database { get; }

        public CrewSkill(Database database)
        {
            this.Database = database;
        }

        public bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "userString":
                    reader.ReadLocalized(this.Database, out _name);
                    return true;
                case "description":
                    string description;
                    reader.ReadLocalized(this.Database, out description);
                    this.ParseDescription(description);
                    return true;
                case "icon":
                    reader.Read(out _icon);
                    return true;
                case SectionDeserializableImpl.TitleToken:
                    return false;
                default:
                    double value;
                    reader.Read(out value);
                    this.Parameters.Add(name, value);
                    return true;
            }
        }

        private void ParseDescription(string description)
        {
            const string shortDescTag = "<shortDesc>";
            const string shortDescTagEnd = "</shortDesc>";
            var startIndex = description.IndexOf(shortDescTag);
            var endIndex = description.IndexOf(shortDescTagEnd);
            if (startIndex >= 0 && endIndex - startIndex - shortDescTag.Length > 0)                
            {
                _shortDescription = description.Substring(startIndex + shortDescTag.Length, endIndex - startIndex - shortDescTag.Length);
                _description = description.Substring(0, startIndex) + this.ShortDescription + description.Substring(endIndex + shortDescTagEnd.Length);
            }
            else
            {
                _shortDescription = _description = description;
            }

        }

        public bool IsWrapped => true;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }


    }
}
