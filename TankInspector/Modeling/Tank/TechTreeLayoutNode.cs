using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    [DebuggerDisplay("{TankKey}")]
    internal class TechTreeLayoutNode : ISectionDeserializable
    {
	    public string TankKey { get; private set; }

	    private int _row;
        public int Row => _row;

	    private int _column;
        public int Column => _column;

	    private readonly List<string> _unlockTanks;
        public IEnumerable<string> UnlockTanks => _unlockTanks;

	    public TechTreeLayoutNode()
        {
            _unlockTanks = new List<string>();
        }

        public TechTreeLayoutNode(IEnumerable<string> unlockTanks)
        {
            if (unlockTanks == null)
                _unlockTanks = new List<string>();
            else
                _unlockTanks = new List<string>(unlockTanks);
        }

        internal TechTreeLayoutNode(string tankKey, int row, int column, IEnumerable<string> unlockTanks)
            : this(unlockTanks)
        {
            this.TankKey = tankKey;
            _row = row;
            _column = column;
        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case SectionDeserializableImpl.TitleToken:
                    this.TankKey = reader.Name;
                    return true;

                case "row":
                    reader.Read(out _row);
                    return true;
                case "column":
                    reader.Read(out _column);
                    return true;
                case "lines":
                    while (reader.IsStartElement())
                    {
                        _unlockTanks.Add(reader.Name);
                        reader.Skip();
                    }
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => true;
    }
}
