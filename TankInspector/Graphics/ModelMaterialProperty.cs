using System.Linq;
using Smellyriver.TankInspector.Modeling;
using SharpDX;
using System.Xml;
using System.Globalization;

namespace Smellyriver.TankInspector.Graphics
{
    public class ModelMaterialProperty : ISectionDeserializable
    {
        private string _name;

        public string Name
        {
            get => _name;
	        set => _name = value;
        }

        private string _texture;

        public string Texture
        {
            get => _texture;
	        set => _texture = value;
        }

        private int _intValue;

        public int IntValue
        {
            get => _intValue;
	        set => _intValue = value;
        }

        private bool _boolValue;

        public bool BoolValue
        {
            get => _boolValue;
	        set => _boolValue = value;
        }

        private float _floatValue;

        public float FloatValue
        {
            get => _floatValue;
	        set => _floatValue = value;
        }

        private Vector4 _vector4Value;

        public Vector4 Vector4Value
        {
            get => _vector4Value;
	        set => _vector4Value = value;
        }
        
        
        public bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case null:
                    reader.Read(out _name);
                    return true;
                case "Texture":
                    reader.Read(out _texture);                 
                    return true;
                case "Int":
                    reader.Read(out _intValue);                 
                    return true;
                case "Bool":
                    try
                    {
                        reader.Read(out _boolValue);
                    }
                    catch
                    {
                        _boolValue = false;
                    }
                    return true;
                case "Float":
                    reader.Read(out _floatValue);
                    return true;
                case "Vector4":
                    reader.Read(out _vector4Value);
                    return true;
                case "<<<<TITLE>>>>":
                    return false;
                default:
                    return false;
            }

        }

        public bool IsWrapped => false;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }
    }

	internal static class XmlReaderExtensions
    {
        public static void Read(this XmlReader reader, out Vector4 value)
        {
            var values = reader.ReadString().Split(null);

            values = values.Where(a => a != "").ToArray();

            value.X = float.Parse(values[0], CultureInfo.InvariantCulture);
            value.Y = float.Parse(values[1], CultureInfo.InvariantCulture);
            value.Z = float.Parse(values[2], CultureInfo.InvariantCulture);
            value.W = float.Parse(values[3], CultureInfo.InvariantCulture);
        }
    }
}
