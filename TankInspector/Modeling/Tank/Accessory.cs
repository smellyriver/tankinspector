using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal abstract class Accessory : Commodity, ISectionDeserializable
    {

        private string _name;
        public string Name => _name;

	    private string _description;
        public string Description => _description;

	    public string Icon { get; private set; }

	    private VehicleFilter _vehicleFilter;
        public VehicleFilter VehicleFilter => _vehicleFilter;

	    public Accessory(Database database)
            : base(database)
        {

        }

        protected override bool DeserializeSection(string name, XmlReader reader)
        {

            switch (name)
            {
                case "userString":
                    reader.ReadLocalized(this.Database, out _name);
                    return true;
                case "description":
                    reader.ReadLocalized(this.Database, out _description);
                    return true;
                case "icon":
                    string icon;
                    reader.Read(out icon);
                    if (icon.EndsWith(" 0 0"))
                        this.Icon = icon.Substring(0, icon.Length - 4);
                    else
                        this.Icon = icon;
                    return true;
                case "vehicleFilter":
                    reader.Read(out _vehicleFilter);
                    return true;
                case "id":
                    return false;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }


        protected override bool IsWrapped => true;

	    public bool CanBeUsedBy(ITank tank)
        {
            return _vehicleFilter == null ? true : _vehicleFilter.Allows(tank);
        }

        public abstract double GetWeight(double tankWeight);
    }
}
