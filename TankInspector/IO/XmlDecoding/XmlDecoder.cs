using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Smellyriver.TankInspector.IO.XmlDecoding
{
    public static class XmlDecoder
    {
        public const int BinaryHeader = 0x42a14e65;
        private const string DigitTagMarker = "____DIGIT____";

        public const string RootNoodName = "BigWorldXml";

        public static string Decode(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                Int32 header = reader.ReadInt32();
                if (header == PackedSection.PackedHeader)
                {
                    reader.ReadSByte();

                    var document = new XmlDocument();

                    var packedSection = new PackedSection();
                    var dictionary = packedSection.ReadDictionary(reader);
                    var xmlroot = document.CreateNode(XmlNodeType.Element, "FromStream", "");
                    packedSection.ReadElement(reader, xmlroot, document, dictionary);
                    document.AppendChild(xmlroot);

                    return XmlDecoder.FormatXml(document.OuterXml);
                }
	            if (header == BinaryHeader)
	            {
		            stream.Seek(0, SeekOrigin.Begin);

		            var document = new XmlDocument();

		            var xmlPrimitives = document.CreateNode(XmlNodeType.Element, "primitives", "");
		            var primitiveReader = new PrimitiveReader();
		            primitiveReader.ReadPrimitives(reader, xmlPrimitives, document);
		            document.AppendChild(xmlPrimitives);

		            return XmlDecoder.FormatXml(document.OuterXml);
	            }
	            using (var streamReader = new StreamReader(stream))
	            {
		            return streamReader.ReadToEnd();
	            }
            }
        }

        public static string Decode(string path)
        {
            var packedFileName = Path.GetFileName(path).ToLower();

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Int32 header = reader.ReadInt32();
                    if (header == PackedSection.PackedHeader)
                    {
                        reader.ReadSByte();

                        var document = new XmlDocument();

                        var packedSection = new PackedSection();
                        var dictionary = packedSection.ReadDictionary(reader);
                        var xmlroot = document.CreateNode(XmlNodeType.Element, packedFileName, "");
                        packedSection.ReadElement(reader, xmlroot, document, dictionary);
                        document.AppendChild(xmlroot);

                        return XmlDecoder.FormatXml(document.OuterXml);
                    }
	                if (header == BinaryHeader)
	                {
		                stream.Seek(0, SeekOrigin.Begin);

		                var document = new XmlDocument();

		                var xmlPrimitives = document.CreateNode(XmlNodeType.Element, "primitives", "");
		                var primitiveReader = new PrimitiveReader();
		                primitiveReader.ReadPrimitives(reader, xmlPrimitives, document);
		                document.AppendChild(xmlPrimitives);

		                return XmlDecoder.FormatXml(document.OuterXml);
	                }
	                var extension = Path.GetExtension(path);
	                if (extension == ".xml" || extension == ".def" || extension == ".visual" || extension == ".chunk" || extension == ".settings" || extension == ".model"
	                    || extension == ".visual_processed")
		                return File.ReadAllText(path);
	                throw new NotSupportedException();
                }
            }

        }

        private static string FormatXml(string sUnformattedXml)
        {
            if (Regex.Match(sUnformattedXml, @"^\<\d").Success)
            {
                var firstTag = sUnformattedXml.Substring(1, sUnformattedXml.IndexOf(">") - 1);
                var tagLength = firstTag.Length;
                firstTag = DigitTagMarker + firstTag;

                sUnformattedXml = string.Format("<{0}>{1}</{0}>", firstTag, sUnformattedXml.Substring(2 + tagLength, sUnformattedXml.Length - tagLength * 2 - 5));

            }

            //load unformatted xml into a dom

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(sUnformattedXml);

            //will hold formatted xml

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");

            //pumps the formatted xml into the StringBuilder above

            StringWriter sw = new StringWriter(sb);

            //does the formatting

            XmlTextWriter xtw = null;

            try
            {
                //point the xtw at the StringWriter

                xtw = new XmlTextWriter(sw);

                //we want the output formatted

                xtw.Formatting = Formatting.Indented;

                //get the dom to dump its contents into the xtw 

                xd.WriteTo(xtw);
            }
            finally
            {
                //clean up even if error

                if (xtw != null)
                    xtw.Close();
            }

            //return the formatted xml

            var result = sb.ToString();

            if (result.StartsWith(DigitTagMarker))
            {
                var firstTag = result.Substring(1, result.IndexOf(">") - 1);
                var tagLength = firstTag.Length;
                firstTag = firstTag.Substring(DigitTagMarker.Length);

                result = string.Format("<{0}>{1}</{0}>", firstTag, result.Substring(2 + tagLength, result.Length - tagLength * 2 - 5));

            }

            return result;
        }


    }
}
