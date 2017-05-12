using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Smellyriver.TankInspector.IO.XmlDecoding
{
    public class PackedSection
    {
        public static readonly Int32 PackedHeader = 0x62a14e45;
        public static readonly char[] IntToBase64 = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };
        public const int MaxLength = 256;

        public class DataDescriptor
        {
            public readonly int Address;
            public readonly int End;
            public readonly int Type;

            public DataDescriptor(int end, int type, int address)
            {
                this.End = end;
                this.Type = type;
                this.Address = address;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("[");
                sb.Append("0x");
                sb.Append(Convert.ToString(End, 16));
                sb.Append(", ");
                sb.Append("0x");
                sb.Append(Convert.ToString(Type, 16));
                sb.Append("]@0x");
                sb.Append(Convert.ToString(Address, 16));
                return sb.ToString();
            }
        }

        public class ElementDescriptor
        {
            public readonly int NameIndex;
            public readonly DataDescriptor DataDescriptor;

            public ElementDescriptor(int nameIndex, DataDescriptor dataDescriptor)
            {
                this.NameIndex = nameIndex;
                this.DataDescriptor = dataDescriptor;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("[");
                sb.Append("0x");
                sb.Append(Convert.ToString(NameIndex, 16));
                sb.Append(":");
                sb.Append(DataDescriptor);
                return sb.ToString();
            }
        }

        public string ReadStringTillZero(BinaryReader reader)
        {
            char[] work = new char[MaxLength];

            int i = 0;

            char c = reader.ReadChar();
            while (c != Convert.ToChar(0x00))
            {
                work[i++] = c;
                c = reader.ReadChar();
            }
            return new string(work, 0, i);

        }

        public List<string> ReadDictionary(BinaryReader reader)
        {
            List<string> dictionary = new List<string>();
            int counter = 0;
            string text = this.ReadStringTillZero(reader);

            while (!(text.Length == 0))
            {
                dictionary.Add(text);
                text = this.ReadStringTillZero(reader);
                counter++;
            }
            return dictionary;
        }

        public int ReadLittleEndianShort(BinaryReader reader)
        {
            int littleEndianShort = reader.ReadInt16();
            return littleEndianShort;
        }

        public int ReadLittleEndianInt(BinaryReader reader)
        {
            int littleEndianInt = reader.ReadInt32();
            return littleEndianInt;
        }

        public DataDescriptor ReadDataDescriptor(BinaryReader reader)
        {
            int selfEndAndType = this.ReadLittleEndianInt(reader);
            return new DataDescriptor(selfEndAndType & 0x0fffffff, selfEndAndType >> 28, (int)reader.BaseStream.Position);
        }

        public ElementDescriptor[] ReadElementDescriptors(BinaryReader reader, int number)
        {
            ElementDescriptor[] elements = new ElementDescriptor[number];
            for (int i = 0; i < number; i++)
            {
                int nameIndex = this.ReadLittleEndianShort(reader);
                DataDescriptor dataDescriptor = this.ReadDataDescriptor(reader);
                elements[i] = new ElementDescriptor(nameIndex, dataDescriptor);
            }
            return elements;
        }

        public string ReadString(BinaryReader reader, int lengthInBytes)
        {
            string rString = new string(reader.ReadChars(lengthInBytes), 0, lengthInBytes);

            return rString;
        }

        public string ReadNumber(BinaryReader reader, int lengthInBytes)
        {
            string number = "";
            switch (lengthInBytes)
            {
                case 1:
                    number = Convert.ToString(reader.ReadSByte(), CultureInfo.InvariantCulture);
                    break;
                case 2:
                    number = Convert.ToString(this.ReadLittleEndianShort(reader), CultureInfo.InvariantCulture);
                    break;
                case 4:
                    number = Convert.ToString(this.ReadLittleEndianInt(reader), CultureInfo.InvariantCulture);
                    break;
                default:
                    number = "0";
                    break;
            }
            return number;

        }

        public float ReadLittleEndianFloat(BinaryReader reader)
        {
            float littleEndianFloat = reader.ReadSingle();
            return littleEndianFloat;
        }

        public string ReadFloats(BinaryReader reader, int lengthInBytes)
        {
            int n = lengthInBytes / 4;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {

                if (i != 0)
                {
                    sb.Append(" ");
                }
                float rFloat = this.ReadLittleEndianFloat(reader);
                sb.Append(rFloat.ToString("0.000000", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }


        public bool ReadBoolean(BinaryReader reader, int lengthInBytes)
        {
            bool @bool = lengthInBytes == 1;
            if (@bool)
            {
                if (reader.ReadSByte() != 1)
                {
                    throw new ArgumentException("Boolean error");
                }
            }

            return @bool;
        }

        private static string ByteArrayToBase64(sbyte[] a)
        {
            int aLen = a.Length;
            int numFullGroups = aLen / 3;
            int numBytesInPartialGroup = aLen - 3 * numFullGroups;
            int resultLen = 4 * ((aLen + 2) / 3);
            StringBuilder result = new StringBuilder(resultLen);

            int inCursor = 0;
            for (int i = 0; i < numFullGroups; i++)
            {
                int byte0 = a[inCursor++] & 0xff;
                int byte1 = a[inCursor++] & 0xff;
                int byte2 = a[inCursor++] & 0xff;
                result.Append(IntToBase64[byte0 >> 2]);
                result.Append(IntToBase64[(byte0 << 4) & 0x3f | (byte1 >> 4)]);
                result.Append(IntToBase64[(byte1 << 2) & 0x3f | (byte2 >> 6)]);
                result.Append(IntToBase64[byte2 & 0x3f]);
            }

            if (numBytesInPartialGroup != 0)
            {
                int byte0 = a[inCursor++] & 0xff;
                result.Append(IntToBase64[byte0 >> 2]);
                if (numBytesInPartialGroup == 1)
                {
                    result.Append(IntToBase64[(byte0 << 4) & 0x3f]);
                    result.Append("==");
                }
                else
                {
                    int byte1 = a[inCursor++] & 0xff;
                    result.Append(IntToBase64[(byte0 << 4) & 0x3f | (byte1 >> 4)]);
                    result.Append(IntToBase64[(byte1 << 2) & 0x3f]);
                    result.Append('=');
                }
            }

            return result.ToString();
        }

        public string ReadBase64(BinaryReader reader, int lengthInBytes)
        {
            sbyte[] bytes = new sbyte[lengthInBytes];
            for (int i = 0; i < lengthInBytes; i++)
            {
                bytes[i] = reader.ReadSByte();
            }
            return PackedSection.ByteArrayToBase64(bytes);
        }

        public string ReadAndToHex(BinaryReader reader, int lengthInBytes)
        {
            sbyte[] bytes = new sbyte[lengthInBytes];
            for (int i = 0; i < lengthInBytes; i++)
            {
                bytes[i] = reader.ReadSByte();
            }
            StringBuilder sb = new StringBuilder("[ ");
            foreach (byte b in bytes)
            {
                sb.Append(Convert.ToString((b & 0xff), 16));
                sb.Append(" ");
            }
            sb.Append("]L:");
            sb.Append(lengthInBytes);

            return sb.ToString();
        }

        public int ReadData(BinaryReader reader, List<string> dictionary, XmlNode element, XmlDocument xDoc, int offset, DataDescriptor dataDescriptor)
        {
            int lengthInBytes = dataDescriptor.End - offset;
            if (dataDescriptor.Type == 0x0)
            {
                // Element                
                this.ReadElement(reader, element, xDoc, dictionary);
            }
            else if (dataDescriptor.Type == 0x1)
            {
                // String
                element.InnerText = this.ReadString(reader, lengthInBytes);

            }
            else if (dataDescriptor.Type == 0x2)
            {
                // Integer number
                element.InnerText = "\t" + this.ReadNumber(reader, lengthInBytes) + "\t";
            }
            else if (dataDescriptor.Type == 0x3)
            {
                // Floats
                string str = this.ReadFloats(reader, lengthInBytes);

                string[] strData = str.Split(' ');
                if (strData.Length == 12)
                {
                    XmlNode row0 = xDoc.CreateElement("row0");
                    XmlNode row1 = xDoc.CreateElement("row1");
                    XmlNode row2 = xDoc.CreateElement("row2");
                    XmlNode row3 = xDoc.CreateElement("row3");
                    row0.InnerText = "\t" + strData[0] + " " + strData[1] + " " + strData[2] + "\t";
                    row1.InnerText = "\t" + strData[3] + " " + strData[4] + " " + strData[5] + "\t";
                    row2.InnerText = "\t" + strData[6] + " " + strData[7] + " " + strData[8] + "\t";
                    row3.InnerText = "\t" + strData[9] + " " + strData[10] + " " + strData[11] + "\t";
                    element.AppendChild(row0);
                    element.AppendChild(row1);
                    element.AppendChild(row2);
                    element.AppendChild(row3);
                }
                else
                {
                    element.InnerText = "\t" + str + "\t";
                }
            }
            else if (dataDescriptor.Type == 0x4)
            {
                // Boolean

                if (this.ReadBoolean(reader, lengthInBytes))
                {
                    element.InnerText = "\ttrue\t";
                }
                else
                {
                    element.InnerText = "\tfalse\t";
                }

            }
            else if (dataDescriptor.Type == 0x5)
            {
                // Base64
                element.InnerText = "\t" + this.ReadBase64(reader, lengthInBytes) + "\t";
            }
            else
            {
                throw new ArgumentException("Unknown type of \"" + element.Name + ": " + dataDescriptor.ToString() + " " + this.ReadAndToHex(reader, lengthInBytes));
            }

            return dataDescriptor.End;
        }

        public void ReadElement(BinaryReader reader, XmlNode element, XmlDocument xDoc, List<string> dictionary)
        {
            int childrenNmber = this.ReadLittleEndianShort(reader);
            DataDescriptor selfDataDescriptor = this.ReadDataDescriptor(reader);
            ElementDescriptor[] children = this.ReadElementDescriptors(reader, childrenNmber);

            int offset = this.ReadData(reader, dictionary, element, xDoc, 0, selfDataDescriptor);

            foreach (ElementDescriptor elementDescriptor in children)
            {
                XmlNode child = xDoc.CreateElement(dictionary[elementDescriptor.NameIndex]);
                offset = this.ReadData(reader, dictionary, child, xDoc, offset, elementDescriptor.DataDescriptor);
                element.AppendChild(child);
            }

        }
    }
}
