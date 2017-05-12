using Smellyriver.TankInspector.Modeling;


namespace Smellyriver.TankInspector.Graphics
{
    public class ModelPrimitiveGroup : ModelSectionDeserializable
    {
        private int _id;

        public int Id
        {
            get => _id;
	        set => _id = value;
        }

        private ModelMaterial _material;

        public ModelMaterial Material
        {
            get => _material;
	        set => _material = value;
        }

        public uint StartIndex { get; set; }


        public uint StartVertex { get; set; }

        public uint EndIndex { get; set; }

        public uint EndVertex { get; set; }

        public override bool DeserializeSection(string name, System.Xml.XmlReader reader)
        {
            switch (name)
            {
                case null:
                    reader.Read(out _id);
                    return true;
                case "material":
                    reader.Read(out _material);
                    return true;
                case "<<<<TITLE>>>>":
                    return false;
                default:
                    return false;
            }
        }

        public bool Sectioned { get; set; }

        public uint PrimitiveCount { get; set; }

        public uint VerticesCount { get; set; }
    }
}
