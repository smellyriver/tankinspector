using System.Globalization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    public static class XmlReaderExtensions
    {
        public static void Read(this XmlReader reader, out double value)
        {
	        var stringValue = reader.ReadString();
	        value = string.IsNullOrEmpty(stringValue) ? default(double) : double.Parse(stringValue, CultureInfo.InvariantCulture);
        }

        public static void Read(this XmlReader reader, out float value)
        {
	        var stringValue = reader.ReadString();
	        value = string.IsNullOrEmpty(stringValue) ? default(float) : float.Parse(stringValue, CultureInfo.InvariantCulture);
        }

        public static void Read(this XmlReader reader, out int value)
        {
	        var stringValue = reader.ReadString();
	        value = string.IsNullOrEmpty(stringValue) ? default(int) : int.Parse(stringValue, CultureInfo.InvariantCulture);
        }

        public static void Read(this XmlReader reader, out uint value)
        {
	        var stringValue = reader.ReadString();
	        value = string.IsNullOrEmpty(stringValue) ? default(uint) : uint.Parse(stringValue, CultureInfo.InvariantCulture);
        }

        public static void Read(this XmlReader reader, out bool value)
        {
	        var stringValue = reader.ReadString();
	        value = !string.IsNullOrEmpty(stringValue) && bool.Parse(stringValue);
        }

        public static void Read(this XmlReader reader, out string value)
        {
            value = reader.ReadString().Trim();
        }

        internal static void ReadLocalized(this XmlReader reader, Database database, out string value)
        {
            value = database.TextData.GetText(reader.ReadString());
        }

        public static void Read(this XmlReader reader, out int price, out CurrencyType currencyType)
        {
            if (!reader.IsStartElement())
            {
				reader.Read(out double doublePrice);
				price = (int)doublePrice;
            }
            else
                price = 0;

            if (reader.IsStartElement("gold"))
            {
                reader.ReadStartElement();
                reader.Skip();
                currencyType = CurrencyType.Gold;
            }
            else
                currencyType = CurrencyType.Credit;
        }

        public static void Read<T>(this XmlReader reader, out T value)
            where T : IDeserializable, new()
        {
            value = new T();
            value.Deserialize(reader);
        }

    }
}
