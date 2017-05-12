using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
/*
 * Kons 2012-12-03 Version .1
 * 
 * Supported features:
 * - DXT1
 * - DXT5
 * - LinearImage (untested)
 * 
 * http://code.google.com/p/kprojects/
 * Send me any change/improvement at kons.snok<at>gmail.com
 */
#pragma warning disable 675
namespace Smellyriver.TankInspector.Graphics
{
	internal class DdsBitmap
    {

        public int Width { get; set; }
        public int Height { get; set; }

        public DdsBitmap(int w, int h)
        {
            // TODO: Complete member initialization
            this.Width = w;
            this.Height = h;
            _data = new int[w * h];
        }

	    private readonly int[] _data;

        //@hillin: commented to prevent warnings
        //internal void SetPixel(int x, int y, Color finalColor)
        //{
        //    data[x + y * Width] = finalColor.bgra;
        //}

        internal void SetPixel(int x, int y, int finalColor)
        {
            _data[x + y * Width] = finalColor;
        }

        public BitmapSource ToBitmapSource()
        {
            var bitmapSource = BitmapSource.Create(Width, Height, 0, 0, PixelFormats.Bgra32, null, _data, Width * sizeof(int));

            //Guid photoID = System.Guid.NewGuid();
            //string photolocation = photoID.ToString() + ".png";  //file name
            //FileStream filestream = new FileStream(photolocation, FileMode.Create);
            //var encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            //encoder.Save(filestream);

            return bitmapSource;
        }
    }

	internal struct Color
    {
        //@hillin: commented to prevent warnings
        //public Int32 bgra;


        public unsafe static int FromArgb(byte a, byte r, byte g, byte b)
        {
            byte* bytes = stackalloc byte[4];

            bytes[0] = b;
            bytes[1] = g;
            bytes[2] = r;
            bytes[3] = a;

            return *(((int*)bytes));
        }
    }

    public class DdsImage
    {
        private const int DdpfAlphapixels = 0x00000001;
        private const int DdpfAlpha = 0x00000002;
        private const int DdpfFourcc = 0x00000004;
        private const int DdpfRgb = 0x00000040;
        private const int DdpfYuv = 0x00000200;
        private const int DdpfLuminance = 0x00020000;
        private const int DdsdMipmapcount = 0x00020000;
        private const int FourccDxt1 = 0x31545844;
        private const int FourccDx10 = 0x30315844;
        private const int FourccDxt5 = 0x35545844;
        private const int FourccDxt4 = 0x33545844;


        public int DwMagic;
        private readonly DdsHeader _header = new DdsHeader();

        //@hillin: commented to prevent warnings
        //private DDS_HEADER_DXT10 header10 = null;//If the DDS_PIXELFORMAT dwFlags is set to DDPF_FOURCC and dwFourCC is set to "DX10"
        public byte[] Bdata;//pointer to an array of bytes that contains the main surface data. 
        public byte[] Bdata2;//pointer to an array of bytes that contains the remaining surfaces such as; mipmap levels, faces in a cube map, depths in a volume texture.

        private readonly DdsBitmap[] _images;


        public BitmapSource BitmapSource(int level)
        {

            if (_images.Length > level)
                return _images[level].ToBitmapSource();
            return null;

        }


        public DdsImage(Stream stream)
        {
            using (BinaryReader r = new BinaryReader(stream))
            {
                DwMagic = r.ReadInt32();
                if (DwMagic != 0x20534444)
                {
                    throw new Exception("This is not a DDS!");
                }

                Read_DDS_HEADER(_header, r);

                if (((_header.Ddspf.DwFlags & DdpfFourcc) != 0) && (_header.Ddspf.DwFourCc == FourccDx10 /*DX10*/))
                {
                    throw new Exception("DX10 not supported yet!");
                }

                int mipMapCount = 1;
                //only need top level
                //if ((header.dwFlags & DDSD_MIPMAPCOUNT) != 0)
                //    mipMapCount = header.dwMipMapCount;
                _images = new DdsBitmap[mipMapCount];

                // The D3DX library (for example, D3DX11.lib) and other similar libraries unreliably or inconsistently provide the pitch value in the dwPitchOrLinearSize member of the DDS_HEADER structure. Therefore, when you read and write to DDS files, we recommend that you compute the pitch in one of the following ways for the indicated formats:
                //For block-compressed formats, compute the pitch as:
                //max( 1, ((width+3)/4) ) * block-size
                //The block-size is 8 bytes for DXT1, BC1, and BC4 formats, and 16 bytes for other block-compressed formats.
                //((width+1) >> 1) * 4
                //For other formats, compute the pitch as:
                //( width * bits-per-pixel + 7 ) / 8
                //You divide by 8 for byte alignment.
                if (_header.DwPitchOrLinearSize == 0)
                {
                    int width = _header.DwWidth;
                    int height = _header.DwHeight;

                    var pitchOrLinearSize = 0;

                    if ((_header.Ddspf.DwFlags & DdpfRgb) != 0)
                    {
                        pitchOrLinearSize = width * height * 4;
                    }
                    else
                    {
                        switch (_header.Ddspf.DwFourCc)
                        {
                            case FourccDxt1:
                                pitchOrLinearSize = ((width + 3) / 4) * ((height + 3) / 4) * 8;
                                break;
                            case FourccDxt5:
                            case FourccDxt4:
                                pitchOrLinearSize = ((width + 3) / 4) * ((height + 3) / 4) * 16;
                                break;
                        }
                    }
                    Bdata = r.ReadBytes(pitchOrLinearSize);
                }
                else
                {
                    Bdata = r.ReadBytes(_header.DwPitchOrLinearSize);
                }


                for (int i = 0; i < mipMapCount; ++i)
                {
                    int w = _header.DwWidth / (2 * i + 1);
                    int h = _header.DwHeight / (2 * i + 1);

                    if ((_header.Ddspf.DwFlags & DdpfRgb) != 0)
                    {
                        _images[i] = this.ReadLinearImage(Bdata, w, h);
                    }
                    else if ((_header.Ddspf.DwFlags & DdpfFourcc) != 0)
                    {
                        _images[i] = this.ReadBlockImage(Bdata, w, h);
                    }

                }
            }
        }

        private unsafe DdsBitmap ReadBlockImage(byte[] data, int w, int h)
        {
            fixed (byte* pData = data)
            {
                switch (_header.Ddspf.DwFourCc)
                {
                    case FourccDxt1:

                        return this.UncompressDxt1(pData, w, h);

                    case FourccDxt5:
                        return this.UncompressDxt5(pData, w, h);
                    case FourccDxt4:
                        return this.UncompressDxt5(pData, w, h);
                    default: break;
                }
            }
            throw new Exception($"0x{_header.Ddspf.DwFourCc.ToString("X")} texture compression not implemented.");
        }

        #region DXT1
        private unsafe DdsBitmap UncompressDxt1(byte* data, int w, int h)
        {
            DdsBitmap res = new DdsBitmap(w, h);

            int blockCountX = (w + 3) / 4;
            int blockCountY = (h + 3) / 4;

            for (int j = 0; j < blockCountY; j++)
            {
                //byte[] blockStorage = r.ReadBytes(blockCountX * 16);
                for (int i = 0; i < blockCountX; i++)
                {
                    this.DecompressBlockDxt1(i * 4, j * 4, w, h, data, res);
                    data += 8;
                }
            }

            return res;
        }

        private unsafe void DecompressBlockDxt1(int x, int y, int width, int height, byte* blockStorage, DdsBitmap image)
        {
            ushort color0 = *((ushort*)(blockStorage));
            ushort color1 = *((ushort*)(blockStorage + 2));

            int temp;

            temp = (color0 >> 11) * 255 + 16;
            byte r0 = (byte)((temp / 32 + temp) / 32);
            temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
            byte g0 = (byte)((temp / 64 + temp) / 64);
            temp = (color0 & 0x001F) * 255 + 16;
            byte b0 = (byte)((temp / 32 + temp) / 32);

            temp = (color1 >> 11) * 255 + 16;
            byte r1 = (byte)((temp / 32 + temp) / 32);
            temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
            byte g1 = (byte)((temp / 64 + temp) / 64);
            temp = (color1 & 0x001F) * 255 + 16;
            byte b1 = (byte)((temp / 32 + temp) / 32);

            uint code = *((uint*)(blockStorage + 4));

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int finalColor = 0;
                    byte positionCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

                    if (color0 > color1)
                    {
                        switch (positionCode)
                        {
                            case 0:
                                finalColor = Color.FromArgb(255, r0, g0, b0);
                                break;
                            case 1:
                                finalColor = Color.FromArgb(255, r1, g1, b1);
                                break;
                            case 2:
                                finalColor = Color.FromArgb(255, (byte)((2 * r0 + r1) / 3), (byte)((2 * g0 + g1) / 3), (byte)((2 * b0 + b1) / 3));
                                break;
                            case 3:
                                finalColor = Color.FromArgb(255, (byte)((r0 + 2 * r1) / 3), (byte)((g0 + 2 * g1) / 3), (byte)((b0 + 2 * b1) / 3));
                                break;
                        }
                    }
                    else
                    {
                        switch (positionCode)
                        {
                            case 0:
                                finalColor = Color.FromArgb(255, r0, g0, b0);
                                break;
                            case 1:
                                finalColor = Color.FromArgb(255, r1, g1, b1);
                                break;
                            case 2:
                                finalColor = Color.FromArgb(255, (byte)((r0 + r1) / 2), (byte)((g0 + g1) / 2), (byte)((b0 + b1) / 2));
                                break;
                            case 3:
                                finalColor = Color.FromArgb(255, 0, 0, 0);
                                break;
                        }
                    }

                    if (x + i < width && y + j < height)
                        image.SetPixel(x + i, y + j, finalColor);
                }
            }
        }
        #endregion
        #region DXT5
        private unsafe DdsBitmap UncompressDxt5(byte* data, int w, int h)
        {
            DdsBitmap res = new DdsBitmap(w, h);

            int blockCountX = (w + 3) / 4;
            int blockCountY = (h + 3) / 4;


            for (int j = 0; j < blockCountY; j++)
            {
                //byte[] blockStorage = r.ReadBytes(blockCountX * 16);
                for (int i = 0; i < blockCountX; i++)
                {
                    //byte[] blockStorage = r.ReadBytes(16);
                    //DecompressBlockDXT5(i * 4, j * 4, w, blockStorage + i * 16, res);
                    this.DecompressBlockDxt5(i * 4, j * 4, w, h, data, res);
                    data += 16;
                }
            }

            return res;
        }

	    private unsafe void DecompressBlockDxt5(int x, int y, int width, int height, byte* blockStorage, DdsBitmap image)
        {
            byte alpha0 = blockStorage[0];
            byte alpha1 = blockStorage[1];

            ushort alphaCode2 = *((ushort*)(blockStorage + 2));
            uint alphaCode1 = *((uint*)(blockStorage + 4));


            ushort color0 = *((ushort*)(blockStorage + 8));
            ushort color1 = *((ushort*)(blockStorage + 10));

            int temp;

            temp = (color0 >> 11) * 255 + 16;
            byte r0 = (byte)((temp / 32 + temp) / 32);
            temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
            byte g0 = (byte)((temp / 64 + temp) / 64);
            temp = (color0 & 0x001F) * 255 + 16;
            byte b0 = (byte)((temp / 32 + temp) / 32);

            temp = (color1 >> 11) * 255 + 16;
            byte r1 = (byte)((temp / 32 + temp) / 32);
            temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
            byte g1 = (byte)((temp / 64 + temp) / 64);
            temp = (color1 & 0x001F) * 255 + 16;
            byte b1 = (byte)((temp / 32 + temp) / 32);

            uint code = *((uint*)(blockStorage + 12));

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int alphaCodeIndex = 3 * (4 * j + i);
                    int alphaCode;

                    if (alphaCodeIndex <= 12)
                    {
                        alphaCode = (alphaCode2 >> alphaCodeIndex) & 0x07;
                    }
                    else if (alphaCodeIndex == 15)
                    {
                        alphaCode = (int)((alphaCode2 >> 15) | ((alphaCode1 << 1) & 0x06));
                    }
                    else
                    {
                        alphaCode = (int)((alphaCode1 >> (alphaCodeIndex - 16)) & 0x07);
                    }

                    byte finalAlpha;
                    if (alphaCode == 0)
                    {
                        finalAlpha = alpha0;
                    }
                    else if (alphaCode == 1)
                    {
                        finalAlpha = alpha1;
                    }
                    else
                    {
                        if (alpha0 > alpha1)
                        {
                            finalAlpha = (byte)(((8 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 7);
                        }
                        else
                        {
                            if (alphaCode == 6)
                                finalAlpha = 0;
                            else if (alphaCode == 7)
                                finalAlpha = 255;
                            else
                                finalAlpha = (byte)(((6 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 5);
                        }
                    }

                    byte colorCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

                    int finalColor = 0;
                    switch (colorCode)
                    {
                        case 0:
                            finalColor = Color.FromArgb(finalAlpha, r0, g0, b0);
                            break;
                        case 1:
                            finalColor = Color.FromArgb(finalAlpha, r1, g1, b1);
                            break;
                        case 2:
                            finalColor = Color.FromArgb(finalAlpha, (byte)((2 * r0 + r1) / 3), (byte)((2 * g0 + g1) / 3), (byte)((2 * b0 + b1) / 3));
                            break;
                        case 3:
                            finalColor = Color.FromArgb(finalAlpha, (byte)((r0 + 2 * r1) / 3), (byte)((g0 + 2 * g1) / 3), (byte)((b0 + 2 * b1) / 3));
                            break;
                    }

                    if (x + i < width && y + j < height)
                        image.SetPixel(x + i, y + j, finalColor);
                    //image[(y + j)*width + (x + i)] = finalColor;
                }
            }
        }
        #endregion

        private DdsBitmap ReadLinearImage(byte[] data, int w, int h)
        {
            DdsBitmap res = new DdsBitmap(w, h);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            res.SetPixel(x, y, r.ReadInt32());
                        }
                    }
                }
            }
            return res;
        }

        private void Read_DDS_HEADER(DdsHeader h, BinaryReader r)
        {
            h.DwSize = r.ReadInt32();
            h.DwFlags = r.ReadInt32();
            h.DwHeight = r.ReadInt32();
            h.DwWidth = r.ReadInt32();
            h.DwPitchOrLinearSize = r.ReadInt32();
            h.DwDepth = r.ReadInt32();
            h.DwMipMapCount = r.ReadInt32();
            for (int i = 0; i < 11; ++i)
            {
                h.DwReserved1[i] = r.ReadInt32();
            }
            Read_DDS_PIXELFORMAT(h.Ddspf, r);
            h.DwCaps = r.ReadInt32();
            h.DwCaps2 = r.ReadInt32();
            h.DwCaps3 = r.ReadInt32();
            h.DwCaps4 = r.ReadInt32();
            h.DwReserved2 = r.ReadInt32();
        }

        private void Read_DDS_PIXELFORMAT(DdsPixelformat p, BinaryReader r)
        {
            p.DwSize = r.ReadInt32();
            p.DwFlags = r.ReadInt32();
            p.DwFourCc = r.ReadInt32();
            p.DwRgbBitCount = r.ReadInt32();
            p.DwRBitMask = r.ReadInt32();
            p.DwGBitMask = r.ReadInt32();
            p.DwBBitMask = r.ReadInt32();
            p.DwABitMask = r.ReadInt32();
        }
    }

	internal class DdsHeader
    {
        public int DwSize;
        public int DwFlags;
        /*	DDPF_ALPHAPIXELS   0x00000001 
            DDPF_ALPHA   0x00000002 
            DDPF_FOURCC   0x00000004 
            DDPF_RGB   0x00000040 
            DDPF_YUV   0x00000200 
            DDPF_LUMINANCE   0x00020000 
         */
        public int DwHeight;
        public int DwWidth;
        public int DwPitchOrLinearSize;
        public int DwDepth;
        public int DwMipMapCount;
        public int[] DwReserved1 = new int[11];
        public DdsPixelformat Ddspf = new DdsPixelformat();
        public int DwCaps;
        public int DwCaps2;
        public int DwCaps3;
        public int DwCaps4;
        public int DwReserved2;
    }

    //@hillin: commented to prevent warnings
    //class DDS_HEADER_DXT10
    //{
    //    public DXGI_FORMAT dxgiFormat;
    //    public D3D10_RESOURCE_DIMENSION resourceDimension;
    //    public uint miscFlag;
    //    public uint arraySize;
    //    public uint reserved;
    //}

	internal class DdsPixelformat
    {
        public int DwSize;
        public int DwFlags;
        public int DwFourCc;
        public int DwRgbBitCount;
        public int DwRBitMask;
        public int DwGBitMask;
        public int DwBBitMask;
        public int DwABitMask;

        public DdsPixelformat()
        {
        }
    }

	internal enum DxgiFormat : uint
    {
        DxgiFormatUnknown = 0,
        DxgiFormatR32G32B32A32Typeless = 1,
        DxgiFormatR32G32B32A32Float = 2,
        DxgiFormatR32G32B32A32Uint = 3,
        DxgiFormatR32G32B32A32Sint = 4,
        DxgiFormatR32G32B32Typeless = 5,
        DxgiFormatR32G32B32Float = 6,
        DxgiFormatR32G32B32Uint = 7,
        DxgiFormatR32G32B32Sint = 8,
        DxgiFormatR16G16B16A16Typeless = 9,
        DxgiFormatR16G16B16A16Float = 10,
        DxgiFormatR16G16B16A16Unorm = 11,
        DxgiFormatR16G16B16A16Uint = 12,
        DxgiFormatR16G16B16A16Snorm = 13,
        DxgiFormatR16G16B16A16Sint = 14,
        DxgiFormatR32G32Typeless = 15,
        DxgiFormatR32G32Float = 16,
        DxgiFormatR32G32Uint = 17,
        DxgiFormatR32G32Sint = 18,
        DxgiFormatR32G8X24Typeless = 19,
        DxgiFormatD32FloatS8X24Uint = 20,
        DxgiFormatR32FloatX8X24Typeless = 21,
        DxgiFormatX32TypelessG8X24Uint = 22,
        DxgiFormatR10G10B10A2Typeless = 23,
        DxgiFormatR10G10B10A2Unorm = 24,
        DxgiFormatR10G10B10A2Uint = 25,
        DxgiFormatR11G11B10Float = 26,
        DxgiFormatR8G8B8A8Typeless = 27,
        DxgiFormatR8G8B8A8Unorm = 28,
        DxgiFormatR8G8B8A8UnormSrgb = 29,
        DxgiFormatR8G8B8A8Uint = 30,
        DxgiFormatR8G8B8A8Snorm = 31,
        DxgiFormatR8G8B8A8Sint = 32,
        DxgiFormatR16G16Typeless = 33,
        DxgiFormatR16G16Float = 34,
        DxgiFormatR16G16Unorm = 35,
        DxgiFormatR16G16Uint = 36,
        DxgiFormatR16G16Snorm = 37,
        DxgiFormatR16G16Sint = 38,
        DxgiFormatR32Typeless = 39,
        DxgiFormatD32Float = 40,
        DxgiFormatR32Float = 41,
        DxgiFormatR32Uint = 42,
        DxgiFormatR32Sint = 43,
        DxgiFormatR24G8Typeless = 44,
        DxgiFormatD24UnormS8Uint = 45,
        DxgiFormatR24UnormX8Typeless = 46,
        DxgiFormatX24TypelessG8Uint = 47,
        DxgiFormatR8G8Typeless = 48,
        DxgiFormatR8G8Unorm = 49,
        DxgiFormatR8G8Uint = 50,
        DxgiFormatR8G8Snorm = 51,
        DxgiFormatR8G8Sint = 52,
        DxgiFormatR16Typeless = 53,
        DxgiFormatR16Float = 54,
        DxgiFormatD16Unorm = 55,
        DxgiFormatR16Unorm = 56,
        DxgiFormatR16Uint = 57,
        DxgiFormatR16Snorm = 58,
        DxgiFormatR16Sint = 59,
        DxgiFormatR8Typeless = 60,
        DxgiFormatR8Unorm = 61,
        DxgiFormatR8Uint = 62,
        DxgiFormatR8Snorm = 63,
        DxgiFormatR8Sint = 64,
        DxgiFormatA8Unorm = 65,
        DxgiFormatR1Unorm = 66,
        DxgiFormatR9G9B9E5Sharedexp = 67,
        DxgiFormatR8G8B8G8Unorm = 68,
        DxgiFormatG8R8G8B8Unorm = 69,
        DxgiFormatBc1Typeless = 70,
        DxgiFormatBc1Unorm = 71,
        DxgiFormatBc1UnormSrgb = 72,
        DxgiFormatBc2Typeless = 73,
        DxgiFormatBc2Unorm = 74,
        DxgiFormatBc2UnormSrgb = 75,
        DxgiFormatBc3Typeless = 76,
        DxgiFormatBc3Unorm = 77,
        DxgiFormatBc3UnormSrgb = 78,
        DxgiFormatBc4Typeless = 79,
        DxgiFormatBc4Unorm = 80,
        DxgiFormatBc4Snorm = 81,
        DxgiFormatBc5Typeless = 82,
        DxgiFormatBc5Unorm = 83,
        DxgiFormatBc5Snorm = 84,
        DxgiFormatB5G6R5Unorm = 85,
        DxgiFormatB5G5R5A1Unorm = 86,
        DxgiFormatB8G8R8A8Unorm = 87,
        DxgiFormatB8G8R8X8Unorm = 88,
        DxgiFormatR10G10B10XrBiasA2Unorm = 89,
        DxgiFormatB8G8R8A8Typeless = 90,
        DxgiFormatB8G8R8A8UnormSrgb = 91,
        DxgiFormatB8G8R8X8Typeless = 92,
        DxgiFormatB8G8R8X8UnormSrgb = 93,
        DxgiFormatBc6HTypeless = 94,
        DxgiFormatBc6HUf16 = 95,
        DxgiFormatBc6HSf16 = 96,
        DxgiFormatBc7Typeless = 97,
        DxgiFormatBc7Unorm = 98,
        DxgiFormatBc7UnormSrgb = 99,
        DxgiFormatAyuv = 100,
        DxgiFormatY410 = 101,
        DxgiFormatY416 = 102,
        DxgiFormatNv12 = 103,
        DxgiFormatP010 = 104,
        DxgiFormatP016 = 105,
        DxgiFormat420Opaque = 106,
        DxgiFormatYuy2 = 107,
        DxgiFormatY210 = 108,
        DxgiFormatY216 = 109,
        DxgiFormatNv11 = 110,
        DxgiFormatAi44 = 111,
        DxgiFormatIa44 = 112,
        DxgiFormatP8 = 113,
        DxgiFormatA8P8 = 114,
        DxgiFormatB4G4R4A4Unorm = 115,
        DxgiFormatForceUint = 0xffffffff
    }

	internal enum D3D10ResourceDimension
    {
        D3D10ResourceDimensionUnknown = 0,
        D3D10ResourceDimensionBuffer = 1,
        D3D10ResourceDimensionTexture1D = 2,
        D3D10ResourceDimensionTexture2D = 3,
        D3D10ResourceDimensionTexture3D = 4
    }

}

#pragma warning restore 675
