using System;
using System.IO;

namespace Smellyriver.TankInspector.IO.MoDecoding
{
	internal unsafe class FastBinaryReader : IDisposable
    {
        private static readonly byte[] Buffer = new byte[256];
        //private Stream baseStream;
        public Stream BaseStream { get; }
        public FastBinaryReader(Stream input)
        {
            BaseStream = input;
        }

        public int ReadInt32()
        {
            BaseStream.Read(Buffer, 0, 4);
            fixed (byte* numRef = &(Buffer[0]))
            {
                return *(((int*)numRef));
            }
        }

        public uint ReadUint32()
        {
            BaseStream.Read(Buffer, 0, 4);
            fixed (byte* numRef = &(Buffer[0]))
            {
                return *(((uint*)numRef));
            }
        }

        public void Dispose()
        {
            
        }

        public string ReadString(int length)
        {
            if (length == 0)
                return "";

            byte[] buffer = new byte[length];

            BaseStream.Read(buffer, 0, length);

            return System.Text.Encoding.UTF8.GetString(buffer);
        }
    }
}
