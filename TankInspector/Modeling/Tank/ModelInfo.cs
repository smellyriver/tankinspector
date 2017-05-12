using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class ModelInfo : ISectionDeserializable, ICloneable
    {
        private string _undamaged;
        
        public string Undamaged
        {
            get => _undamaged;
	        set => _undamaged = value;
        }

        private string _destroyed;
        
        public string Destroyed
        {
            get => _destroyed;
	        set => _destroyed = value;
        }

        private string _exploded;
        
        public string Exploded
        {
            get => _exploded;
	        set => _exploded = value;
        }

        private string _collision;
        
        public string Collision
        {
            get => _collision;
	        set => _collision = value;
        }


        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "undamaged":
                    reader.Read(out _undamaged);
                    return true;
                case "destroyed":
                    reader.Read(out _destroyed);
                    return true;
                case "exploded":
                    reader.Read(out _exploded);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;


	    public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
