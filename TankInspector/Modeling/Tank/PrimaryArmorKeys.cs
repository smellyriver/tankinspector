using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class PrimaryArmorKeys : IDeserializable
    {
	    public string Front { get; private set; }

	    public string Side { get; private set; }

	    public string Rear { get; private set; }

	    public PrimaryArmorKeys(string front, string side, string back)
        {
            this.Front = front;
            this.Side = side;
            this.Rear = back;
        }

        public PrimaryArmorKeys()
        {

        }


        public void Deserialize(XmlReader reader)
        {
            var values = reader.ReadString();
            var valuesArray = values.Split(' ');

            this.Front = valuesArray[0];
            this.Side = valuesArray[1];
            this.Rear = valuesArray[2];
        }

    }
}
