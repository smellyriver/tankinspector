using SharpDX;
using System;
using System.Windows.Media.Media3D;

namespace Smellyriver.TankInspector.Graphics.Frameworks
{
    public static class DxUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetOrThrow<T>(this T obj)
            where T : class, IDisposable
        {
            if (obj == null)
                throw new ObjectDisposedException(typeof(T).Name);
            return obj;
        } 

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 TransformNormal(this Matrix m, Vector3 v)
        {            
            var v2 = DxUtils.Multiply(m, v.X, v.Y, v.Z, 0);
            return new Vector3(v2.X, v2.Y, v2.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 TransformCoord(this Matrix m, Vector3 v)
        {
            var v2 = DxUtils.Multiply(m, v.X, v.Y, v.Z, 1);
            return new Vector3(v2.X, v2.Y, v2.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 Multiply(this Matrix m, float x, float y, float z, float w)
        {
            return new Vector3(
                m.M11 * x + m.M21 * y + m.M31 * z + m.M41 * w
                , m.M12 * x + m.M22 * y + m.M32 * z + m.M42 * w
                , m.M13 * x + m.M23 * y + m.M33 * z + m.M43 * w
                );
        }

        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }

        /// <summary>
        /// 
        /// </summary>
        public static float Deg2Rad(this float degrees)
        {
            return degrees * (float)Math.PI / 180.0f;
        }

        public static Vector3 Convert( Point3D xyz )
        {
            return new Vector3((float)xyz.X, (float)xyz.Y, (float)xyz.Z);
        }

        public static Vector3 Convert(Vector3D xyz)
        {
            return new Vector3((float)xyz.X, (float)xyz.Y, (float)xyz.Z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="flags"></param>
        /// <param name="format"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        //public static Direct3D11.Texture2D CreateTexture2D(this Direct3D11.Device device,
        //    int w, int h,
        //    Direct3D11.BindFlags flags = Direct3D11.BindFlags.RenderTarget | Direct3D11.BindFlags.ShaderResource,
        //    Format format = Format.B8G8R8A8_UNorm,
        //    Direct3D11.ResourceOptionFlags options = Direct3D11.ResourceOptionFlags.Shared)
        //{
        //    var colordesc = new Direct3D11.Texture2DDescription
        //    {
        //        BindFlags = flags,
        //        Format = format,
        //        Width = w,
        //        Height = h,
        //        MipLevels = 1,
        //        SampleDescription = new SampleDescription(1, 0),
        //        Usage = Direct3D11.ResourceUsage.Default,
        //        OptionFlags = options,
        //        CpuAccessFlags = Direct3D11.CpuAccessFlags.None,
        //        ArraySize = 1
        //    };
        //    return new Direct3D11.Texture2D(device, colordesc);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="flags"></param>
        /// <param name="format"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        //public static Direct3D10.Texture2D CreateTexture2D(this Direct3D10.Device device,
        //    int w, int h,
        //    Direct3D10.BindFlags flags = Direct3D10.BindFlags.RenderTarget | Direct3D10.BindFlags.ShaderResource,
        //    Format format = Format.B8G8R8A8_UNorm,
        //    Direct3D10.ResourceOptionFlags options = Direct3D10.ResourceOptionFlags.Shared)
        //{
        //    var colordesc = new Direct3D10.Texture2DDescription
        //    {
        //        BindFlags = flags,
        //        Format = format,
        //        Width = w,
        //        Height = h,
        //        MipLevels = 1,
        //        SampleDescription = new SampleDescription(1, 0),
        //        Usage = Direct3D10.ResourceUsage.Default,
        //        OptionFlags = options,
        //        CpuAccessFlags = Direct3D10.CpuAccessFlags.None,
        //        ArraySize = 1
        //    };
        //    return new Direct3D10.Texture2D(device, colordesc);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        //public static Direct3D11.Buffer CreateBuffer<T>(this Direct3D11.Device device, T[] range)
        //    where T : struct
        //{
        //    int sizeInBytes = Marshal.SizeOf(typeof(T));
        //    using (var stream = new DataStream(range.Length * sizeInBytes, true, true))
        //    {
        //        stream.WriteRange(range);
        //        return new Direct3D11.Buffer(device, stream, new Direct3D11.BufferDescription
        //        {
        //            BindFlags = Direct3D11.BindFlags.VertexBuffer,
        //            SizeInBytes = (int)stream.Length,
        //            CpuAccessFlags = Direct3D11.CpuAccessFlags.None,
        //            OptionFlags = Direct3D11.ResourceOptionFlags.None,
        //            StructureByteStride = 0,
        //            Usage = Direct3D11.ResourceUsage.Default,
        //        });
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        //public static Direct3D10.Buffer CreateBuffer<T>(this Direct3D10.Device device, T[] range)
        //    where T : struct
        //{
        //    int sizeInBytes = Marshal.SizeOf(typeof(T));
        //    using (var stream = new DataStream(range.Length * sizeInBytes, true, true))
        //    {
        //        stream.WriteRange(range);
        //        return new Direct3D10.Buffer(device, stream, new Direct3D10.BufferDescription
        //        {
        //            BindFlags = Direct3D10.BindFlags.VertexBuffer,
        //            SizeInBytes = (int)stream.Length,
        //            CpuAccessFlags = Direct3D10.CpuAccessFlags.None,
        //            OptionFlags = Direct3D10.ResourceOptionFlags.None,
        //            Usage = Direct3D10.ResourceUsage.Default,
        //        });
        //    }
        //}       



        public static Vector2 Convert(System.Windows.Point xy)
        {
            return new Vector2((float)xy.X, (float)xy.Y);
        }
    }
}
