using System;
using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Crew : IDeserializable
    {
        [Stat]
        public CrewRoleType PrimaryRole { get; set; }

        private List<CrewRoleType> _secondaryPositions;

        [ExpandableStat]
        public List<CrewRoleType> SecondaryRoles
        {
            get => _secondaryPositions;
	        set => _secondaryPositions = value;
        }

        public IEnumerable<CrewRoleType> AllRoles
        {
            get
            {
                yield return this.PrimaryRole;
                foreach (var position in this.SecondaryRoles)
                    yield return position;
            }
        }

        public Crew()
        {
            _secondaryPositions = new List<CrewRoleType>();
        }

        public void Deserialize(XmlReader reader)
        {
            this.PrimaryRole = CrewRoleTypeEx.Parse(reader.Name);

            reader.ReadStartElement();
            if (reader.HasValue)
            {
                var secondaryPositions = reader.ReadString();
                var secondaryPositionsArray = secondaryPositions.Split(new[] { '\r', '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var position in secondaryPositionsArray)
                {
                    _secondaryPositions.Add(CrewRoleTypeEx.Parse(position));
                }
            }

            reader.ReadEndElement();
        }

    }
}
