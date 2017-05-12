using System;
using System.Windows;
using System.Windows.Interop;
using SharpDX.Direct3D9;

namespace Smellyriver.TankInspector.Graphics.Frameworks
{
	public class D3D9ImageSource : D3DImage
	{
        /// <summary>
        /// 
        /// </summary>
		public D3D9ImageSource()
		{
        }

        ~D3D9ImageSource() { }


        public void Invalidate()
        {
            AddDirtyRect(new Int32Rect(0, 0, this.PixelWidth, this.PixelHeight));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public void SetBackBuffer(Texture texture)
        {
            if (texture != null)
            {
                using (Surface surface = texture.GetSurfaceLevel(0))
                {
                    Lock();
                    SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
                    AddDirtyRect(new Int32Rect(0, 0, this.PixelWidth, this.PixelHeight));
                    Unlock();
                }
            }
            else
            {
                Lock();
                SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                AddDirtyRect(new Int32Rect(0, 0, this.PixelWidth, this.PixelHeight));
                Unlock();
            }
        }		
	}
}
