using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class VehicleFilter : ISectionDeserializable
    {

        public enum FilterMode
        {
            Include,
            Exclude
        }

        private class VehicleRule : ISectionDeserializable
        {
            public string[] Tags { get; private set; }

	        public int? MinLevel { get; private set; }

	        public int? MaxLevel { get; private set; }


	        public bool IsWrapped => false;

	        public void Deserialize(XmlReader reader)
            {
                SectionDeserializableImpl.Deserialize(this, reader);
            }

            public bool DeserializeSection(string name, XmlReader reader)
            {
                switch (name)
                {
                    case "tags":
                        var tags = reader.ReadString().Trim();
                        this.Tags = tags.Split(' ');
                        return true;
                    case "minLevel":
                        int minLevel;
                        reader.Read(out minLevel);
                        this.MinLevel = minLevel;
                        return true;
                    case "maxLevel":
                        int maxLevel;
                        reader.Read(out maxLevel);
                        this.MaxLevel = maxLevel;
                        return true;
                    case SectionDeserializableImpl.TitleToken:
                        return false;
                    default:
                        return false;
                }
            }
        }

        public class FilterRule : ISectionDeserializable
        {

            public FilterMode FilterMode { get; }

            public HashSet<string> Nations { get; private set; }
            public HashSet<string> VehicleTags { get; private set; }
            public int? MinLevel { get; private set; }
            public int? MaxLevel { get; private set; }

            public FuelType? EngineFuelType { get; private set; }

            public FilterRule(FilterMode mode)
            {
                this.FilterMode = mode;
            }

            public bool IsWrapped => false;

	        public void Deserialize(XmlReader reader)
            {
                SectionDeserializableImpl.Deserialize(this, reader);
            }

            public bool DeserializeSection(string name, XmlReader reader)
            {
                switch (name)
                {
                    case "nations":
                        var nations = reader.ReadString().Trim();
                        if (nations.Length > 0)
                            this.Nations = new HashSet<string>(nations.Split(' '));
                        return true;
                    case "vehicle":
                        VehicleRule vehicleRule;
                        reader.Read(out vehicleRule);
                        if (vehicleRule.Tags != null)
                            this.VehicleTags = new HashSet<string>(vehicleRule.Tags);
                        this.MaxLevel = vehicleRule.MaxLevel;
                        this.MinLevel = vehicleRule.MinLevel;
                        return true;
                    case "engine":
                        reader.ReadStartElement("tags");
                        this.EngineFuelType = FuelTypeEx.Parse(reader.ReadString());
                        reader.ReadEndElement();
                        return true;
                    case SectionDeserializableImpl.TitleToken:
                        return false;
                    default:
                        return false;
                }
            }
        }

        public List<FilterRule> Rules { get; private set; }

        public bool DeserializeSection(string name, XmlReader reader)
        {
            FilterMode mode;

            switch (name)
            {
                case "include":
                    mode = FilterMode.Include;
                    break;
                case "exclude":
                    mode = FilterMode.Exclude;
                    break;
                default:
                    return false;
            }

            var rule = new FilterRule(mode);
            rule.Deserialize(reader);
            this.Rules.Add(rule);
            return true;
        }

        public bool IsWrapped => false;

	    public void Deserialize(XmlReader reader)
        {
            this.Rules = new List<FilterRule>();
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        public bool Allows(ITank tank)
        {
            var allows = false;
            foreach (var rule in this.Rules)
            {
                var matches = true;
                if (rule.Nations != null && !rule.Nations.Contains(tank.NationKey))
                    matches = false;
                else if (rule.MinLevel != null && ((ITankInfo)tank).Tier < rule.MinLevel.Value)
                    matches = false;
                else if (rule.MaxLevel != null && ((ITankInfo)tank).Tier > rule.MaxLevel.Value)
                    matches = false;
                else if (rule.VehicleTags != null && !this.MatchTag(tank.Tags, rule.VehicleTags))
                    matches = false;
                else if (rule.EngineFuelType != null && tank.AvailableEngines.First().Value.FuelType != rule.EngineFuelType.Value)
                    matches = false;

                if (matches)
                {
                    if (rule.FilterMode == FilterMode.Exclude)
                        allows = false;
                    else if (rule.FilterMode == FilterMode.Include)
                        allows = true;
                }
            }

            return allows;
        }

        private bool MatchTag(IEnumerable<string> tags, HashSet<string> reference)
        {
            foreach (var tag in tags)
            {
                if (reference.Contains(tag))
                    return true;
            }

            return false;
        }
    }
}
