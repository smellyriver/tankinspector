using System;
using System.Diagnostics;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    [DebuggerDisplay("{Key}")]
    internal abstract class Commodity : DatabaseObject, ISectionDeserializable, ICommodity
    {

        private string _key;

        public string Key
        {
            get => _key;
	        protected set => _key = value;
        }

        private int _price;

        [Stat("Price", DataAnalysis.ComparisonMode.Plain)]
        public int Price
        {
            get => _price;
	        protected set => _price = value;
        }

        private bool _notInShop;

        public bool NotInShop
        {
            get => _notInShop;
	        protected set => _notInShop = value;
        }

        private CurrencyType _currencyType;

        public CurrencyType CurrencyType
        {
            get => _currencyType;
	        protected set => _currencyType = value;
        }

        public Commodity(Database database)
            : base(database)
        {

        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            return this.DeserializeSection(name, reader);
        }

        protected virtual bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case SectionDeserializableImpl.TitleToken:
                    this.Key = reader.Name;
                    return true;
                case "price":
                    reader.Read(out _price, out _currencyType);
                    return true;
				case "notInShop":
                    reader.Read(out _notInShop);
                    return true;

                default:
                   return false;
            }

        }

        protected virtual bool IsWrapped => true;

	    bool ISectionDeserializable.IsWrapped => this.IsWrapped;


	    public virtual void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }


    }
}
