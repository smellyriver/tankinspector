using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.Graphics
{
    public class ModelRenderSet : ModelSectionDeserializable
    {
        //todo BlendBone
        private Geometry _geometry;

	    public Geometry Geometry
	    {
		    get => _geometry;
		    set => _geometry = value;
	    }

        public ModelVisual Visual{ get; set; }
	

        public override bool DeserializeSection(string name, System.Xml.XmlReader reader)
        {
            switch (name)
            {
                case "geometry":
                    reader.Read(out _geometry);
                    return true;
                case "<<<<TITLE>>>>":
                    return false;
                case "treatAsWorldSpaceObject":
                    return false;
                case "node":
                    return false;
                default:
                    return false;
            }
        }
    }
}
