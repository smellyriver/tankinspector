using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal class TankInfo : Commodity, ITankInfo
    {

        private string _name;
        public string Name => _name;

	    private string _shortName;
        public string ShortName
        {
            get
            {
	            if (string.IsNullOrEmpty(_shortName))
                    return this.Name;
	            return _shortName;
            }
        }

        public string IconKey => $"{this.Nation.Key}-{this.Key}";

	    public string ColonFullKey => $"{this.Nation.Key}:{this.Key}";

	    private string _description;
        public string Description => _description;


	    private int _tier;
        public int Tier => _tier;


	    private uint _id;
        public uint Id => _id;

	    public TankClass Class { get; private set; }

	    public NationalDatabase Nation { get; }

        public string[] Tags { get; private set; }

        public TankInfo(NationalDatabase nation)
            : base(nation.Database)
        {
            this.Nation = nation;
        }


        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "userString":
                    reader.ReadLocalized(this.Database, out _name);
                    return true;
                case "shortUserString":
                    reader.ReadLocalized(this.Database, out _shortName);
                    return true;
                case "description":
                    reader.ReadLocalized(this.Database, out _description);
                    return true;
                case "tags":
                    this.ParseTags(reader.ReadString());
                    return true;
                case "level":
                    reader.Read(out _tier);
                    return true;
                case "id":
                    reader.Read(out _id);
                    return true;
                default:
                    if (base.DeserializeSection(name, reader))
                        return true;
                    else
                        return false;
            }
        }

        private void ParseTags(string tags)
        {
            var tagsArray = tags.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            switch (tagsArray[0])
            {
                case "mediumTank":
                    this.Class = TankClass.MediumTank;
                    break;
                case "lightTank":
                    this.Class = TankClass.LightTank;
                    break;
                case "heavyTank":
                    this.Class = TankClass.HeavyTank;
                    break;
                case "AT-SPG":
                    this.Class = TankClass.TankDestroyer;
                    break;
                case "SPG":
                    this.Class = TankClass.SelfPropelledGun;
                    break;
            }

            this.Tags = tagsArray;
        }

    }
}
