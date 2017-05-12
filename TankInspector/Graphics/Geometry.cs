using System.Collections.Generic;
using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.Graphics
{
    public class Geometry : ModelSectionDeserializable
    {
        private string _verticesName;

        public string VerticesName
        {
            get => _verticesName.Trim();
	        set => _verticesName = value;
        }

        private string _primitiveName;

        public string IndicesName
        {
            get => _primitiveName.Trim();
	        set => _primitiveName = value;
        }

        private string _streamName;

        public string StreamName
        {
            get => _streamName.Trim();
	        set => _streamName = value;
        }
        

        public Dictionary<int, ModelPrimitiveGroup> ModelPrimitiveGroups { get; }

        public Geometry()
        {
            ModelPrimitiveGroups = new Dictionary<int, ModelPrimitiveGroup>();
        }

        public void AddPrimitiveGroup(ModelPrimitiveGroup primitiveGroup)
        {
            ModelPrimitiveGroups.Add(primitiveGroup.Id, primitiveGroup);
        }

        public override bool DeserializeSection(string name, System.Xml.XmlReader reader)
        {
            switch (name)
            {
                case "vertices":
                    reader.Read(out _verticesName);
                    return true;
                case "stream":
                    reader.Read(out _streamName);
                    return true;
                case "primitive":
                    reader.Read(out _primitiveName);
                    return true;
                case "primitiveGroup":
                    ModelPrimitiveGroup group;
                    reader.Read(out group);
                    AddPrimitiveGroup(group);
                    return true;
                default:
                    return false;
            }
        }
#if WITHOUT_D3D
        public PointCollection TextureCoordinates { get; set; }

        public Vector3DCollection Normals { get; set; }

        public Point3DCollection Positions { get; set; }

        public Int32Collection TriangleIndices { get; set; }
#endif
    }
}
