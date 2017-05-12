using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    public struct BattleTierSpan : IXmlSerializable
    {
        public int From { get; set; }
        public int To { get; set; }

        public BattleTierSpan(int lowerBound, int upperBound)
            : this()
        {
            this.From = lowerBound;
            this.To = upperBound;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            var span = reader.ReadString();
            var bounds = span.Split('-');
            this.From = int.Parse(bounds[0],CultureInfo.InvariantCulture);

            if (bounds.Length == 1)
                this.To = this.From;
            else
                this.To = int.Parse(bounds[1], CultureInfo.InvariantCulture);

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (this.From == this.To)
                writer.WriteString(this.From.ToString());
            else
                writer.WriteString($"{this.From}-{this.To}");
        }
    }
}
