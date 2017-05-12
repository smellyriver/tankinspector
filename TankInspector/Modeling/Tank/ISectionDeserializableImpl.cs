using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
	internal static class SectionDeserializableImpl
    {
        public const string TitleToken = "<<<<TITLE>>>>";

        public static void Deserialize(this ISectionDeserializable @this, XmlReader reader)
        {
            @this.DeserializeSection(TitleToken, reader);
            if (@this.IsWrapped)
                reader.ReadStartElement();

            if (reader.NodeType == XmlNodeType.Text)
                if (!@this.DeserializeSection(null, reader))
                    reader.Skip();

            while (reader.IsStartElement())
            {
                var tag = reader.Name;

                reader.ReadStartElement();

                if (!@this.DeserializeSection(tag, reader))
                    while (reader.IsStartElement()  || reader.HasValue)
                        reader.Skip();

                reader.ReadEndElement();

            }

            if (@this.IsWrapped)
                reader.ReadEndElement();
        }
    }
}
