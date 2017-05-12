using System.Collections.Generic;
using Smellyriver.TankInspector.Modeling;


namespace Smellyriver.TankInspector.Graphics
{
    public class ModelMaterial : ModelSectionDeserializable
    {
        private string _identifier;

        public string Identifier
        {
            get => _identifier;
	        set => _identifier = value;
        }

        private string _fx;

        public string Fx
        {
            get => _fx;
	        set => _fx = value;
        }

        private int _collisionFlags;

        public int CollisionFlags
        {
            get => _collisionFlags;
	        set => _collisionFlags = value;
        }

        private int _materialKind;

        public int MaterialKind
        {
            get => _materialKind;
	        set => _materialKind = value;
        }

        public bool ShowArmor { get; set; }

        internal ArmorGroup Armor { get; set; }


        public List<ModelMaterialProperty> Propertys { get; }

        public ModelMaterial()
        {
            Propertys = new List<ModelMaterialProperty>();
        }


        public override bool DeserializeSection(string name, System.Xml.XmlReader reader)
        {
            switch (name)
            {
                case "identifier":
                    reader.Read(out _identifier);
                    return true;
                case "fx":
                    reader.Read(out _fx);
                    return true;
                case "collisionFlags":
                    reader.Read(out _collisionFlags);
                    return true;
                case "materialKind":
                    reader.Read(out _materialKind);
                    return true;
                case "property":
                    ModelMaterialProperty property;
                    reader.Read(out property);
                    Propertys.Add(property);
                    return true;
                case "<<<<TITLE>>>>":
                    return false;
                default:
                    return false;
            }
            
        }




    }
}
