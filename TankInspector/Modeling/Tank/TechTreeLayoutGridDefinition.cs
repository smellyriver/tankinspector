using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{

    [Serializable]
    internal class TechTreeLayoutGridDefinition : ISectionDeserializable
    {

        private int _rows;
        public int Rows => _rows;

	    private int _columns;
        public int Columns => _columns;


	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "rows":
                    reader.Read(out _rows);
                    ++_rows;
                    return true;
                case "columns":
                    reader.Read(out _columns);
                    ++_columns;
                    return false;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
