using System;
using System.Runtime.InteropServices;
using SharpDX.Direct3D9;

namespace Smellyriver.TankInspector.Graphics.Frameworks
{
    public class D3D9 : D3D
    {
        /// <summary>
        /// 
        /// </summary>
        public D3D9()
        {
            PresentParameters presentparams = new PresentParameters();
            presentparams.Windowed = true;
            presentparams.SwapEffect = SwapEffect.Discard;
            presentparams.DeviceWindowHandle = D3D9.GetDesktopWindow();
            presentparams.EnableAutoDepthStencil = false;
            presentparams.AutoDepthStencilFormat = Format.D24S8;
            presentparams.PresentationInterval = PresentInterval.Default;

            try
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    var context = new Direct3DEx();
                    Context = context;
                    this._device = new DeviceEx(context, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
                }
            }
            catch (Exception)
            {
                Context = new Direct3D();
                this._device = new Device(Context, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposer.RemoveAndDispose(ref _device);
                Disposer.RemoveAndDispose(ref Context);
                Disposer.RemoveAndDispose(ref _renderTarget);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed => _device == null;

	    /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = false)]
	    private static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 
        /// </summary>
        public Device Device => _device.GetOrThrow();

	    /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public override void Reset(int w, int h)
        {
            _device.GetOrThrow();

            if (w < 1)
                throw new ArgumentOutOfRangeException("w");
            if (h < 1)
                throw new ArgumentOutOfRangeException("h");

            Disposer.RemoveAndDispose(ref _renderTarget);

            _renderTarget = new Texture(this._device, w, h, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            using (var surface = _renderTarget.GetSurfaceLevel(0))
                _device.SetRenderTarget(0, surface);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        protected T Prepared<T>(ref T property)
        {
            _device.GetOrThrow();
            if (property == null)
                Reset(1, 1);
            return property;
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture RenderTarget => Prepared(ref _renderTarget);

	    /// <summary>
        /// 
        /// </summary>
        /// <param name="dximage"></param>
        public override void SetBackBuffer(D3D9ImageSource dximage) 
        { 
            dximage.SetBackBuffer(RenderTarget); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Windows.Media.Imaging.WriteableBitmap ToImage() { throw new NotImplementedException(); }

        protected Direct3D Context;
        protected Device _device;
        private   Texture _renderTarget;
    }
}
