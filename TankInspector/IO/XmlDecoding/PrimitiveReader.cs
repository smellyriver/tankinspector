using System;
using System.IO;
using System.Xml;

namespace Smellyriver.TankInspector.IO.XmlDecoding
{
    public class PrimitiveReader
    {
        public void ReadPrimitives(BinaryReader reader, XmlNode element, XmlDocument xDoc)
        {
            int len = (int)reader.BaseStream.Length;
            int data = (int)reader.BaseStream.Position + sizeof(int);

            reader.BaseStream.Position = ((int)reader.BaseStream.Position + len - sizeof(int));
            int indexLen = reader.ReadInt32();
            int offset = len - (indexLen + sizeof(int));
            reader.BaseStream.Position = offset;
            long oldDataLen = 4;
            while (offset < (len - 4))
            {
                int entryDataLen = 0;
                int entryNameLen = 0;
                for (int i = 0; i < (len - (int)reader.BaseStream.Position); i++)
                {
                    if (reader.ReadByte() != 0x00)
                    {
                        reader.BaseStream.Position = (int)reader.BaseStream.Position - 1;
                        entryDataLen = reader.ReadInt32();
                        break;
                    }
                }

                for (int i = 0; i < (len - (int)reader.BaseStream.Position); i++)
                {
                    if (reader.ReadByte() != 0x00)
                    {
                        reader.BaseStream.Position = (int)reader.BaseStream.Position - 1;
                        entryNameLen = reader.ReadInt32();
                        break;
                    }
                }

                string entryStr = new string(reader.ReadChars(entryNameLen), 0, entryNameLen);

                XmlNode xentryStr = xDoc.CreateElement("primitive");
                XmlAttribute attr = xDoc.CreateAttribute("id");
                attr.InnerText = entryStr;
                xentryStr.Attributes.Append(attr);

                XmlNode xentryDataPos = xDoc.CreateElement("position");
                xentryDataPos.InnerText = Convert.ToString(oldDataLen);
                XmlNode xentryDataLen = xDoc.CreateElement("length");
                xentryDataLen.InnerText = Convert.ToString(entryDataLen);
                oldDataLen += (entryDataLen + 3) & (~3L);

                xentryStr.AppendChild(xentryDataPos);
                xentryStr.AppendChild(xentryDataLen);

                offset = (int)reader.BaseStream.Position + entryNameLen;
                element.AppendChild(xentryStr);
            }
        }
    }
}
