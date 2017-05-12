using System;
using System.Collections.Generic;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class TechTreeLayout : ISectionDeserializable
    {

        private TechTreeLayoutGridDefinition _gridDefinition;
        public TechTreeLayoutGridDefinition GridDefinition => _gridDefinition;

	    private readonly List<TechTreeLayoutNode> _nodes;
        public IEnumerable<TechTreeLayoutNode> Nodes => _nodes;

	    [NonSerialized]
        private TechTree _techTree;
        internal TechTree TechTree
        {
            get => _techTree;
	        set => _techTree = value;
        }

        public TechTreeLayout(TechTree techTree)
        {
            _techTree = techTree;
            _nodes = new List<TechTreeLayoutNode>();
        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "grid":
                    reader.ReadString();
                    reader.Read(out _gridDefinition);
                    return true;
                case "nodes":
                    while (reader.IsStartElement())
                    {
						reader.Read(out TechTreeLayoutNode node);
						_nodes.Add(node);
                    }
                    return true;
                case "settings":
                    return false;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
