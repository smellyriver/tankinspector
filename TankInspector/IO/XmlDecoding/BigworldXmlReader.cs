using System.IO;
using System.Xml;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.IO.XmlDecoding
{
	internal class BigworldXmlReader : XmlReader
    {
        public override int AttributeCount => _reader.AttributeCount;

	    public override string BaseURI => _reader.BaseURI;

	    public override void Close()
        {
            _reader.Close();
        }

        public override int Depth => _reader.Depth;

	    public override bool EOF => _reader.EOF;

	    public override string GetAttribute(int i)
        {
            return _reader.GetAttribute(i);
        }

        public override string GetAttribute(string name, string namespaceUri)
        {
            return _reader.GetAttribute(name, namespaceUri);
        }

        public override string GetAttribute(string name)
        {
            return _reader.GetAttribute(name);
        }

        public override bool IsEmptyElement => _reader.IsEmptyElement;

	    public override string LocalName => _reader.LocalName;

	    public override string LookupNamespace(string prefix)
        {
            return _reader.LookupNamespace(prefix);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return _reader.MoveToAttribute(name, ns);
        }

        public override bool MoveToAttribute(string name)
        {
            return _reader.MoveToAttribute(name);
        }

        public override bool MoveToElement()
        {
            return _reader.MoveToElement();
        }

        public override bool MoveToFirstAttribute()
        {
            return _reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        public override XmlNameTable NameTable => _reader.NameTable;

	    public override string NamespaceURI => _reader.NamespaceURI;

	    public override XmlNodeType NodeType => _reader.NodeType;

	    public override string Prefix => _reader.Prefix;

	    public override bool Read()
        {
            return _reader.Read();
        }

        public override bool ReadAttributeValue()
        {
            return _reader.ReadAttributeValue();
        }

        public override ReadState ReadState => _reader.ReadState;

	    public override void ResolveEntity()
        {
            _reader.ResolveEntity();
        }

#if DEBUG
	    public string DecodedXml { get; }
#endif

		public override string Value => _reader.Value;

	    private readonly Stream _contentStream;
        private readonly XmlTextReader _reader;

        public BigworldXmlReader(string path)
        {
            var content = XmlDecoder.Decode(path);
#if DEBUG
	        this.DecodedXml = content;
#endif
			_contentStream = content.ToStream();
            _reader = new XmlTextReader(_contentStream);
            _reader.WhitespaceHandling = WhitespaceHandling.None;
        }

        public BigworldXmlReader(Stream contentStream)
        {
            var content = XmlDecoder.Decode(contentStream);
#if DEBUG
			this.DecodedXml = content;
#endif
			_contentStream = content.ToStream();
            _reader = new XmlTextReader(_contentStream);
            _reader.WhitespaceHandling = WhitespaceHandling.None;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _contentStream.Dispose();
            }
        }

    }
}
