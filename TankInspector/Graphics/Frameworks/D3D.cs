using SharpDX;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;

namespace Smellyriver.TankInspector.Graphics.Frameworks
{
    /// <summary>
    /// A vanilla implementation of <see cref="IDirect3D"/> with some common wiring already done.
    /// </summary>
    public abstract class D3D : IDirect3D, IDisposable, INotifyPropertyChanged, IInteractiveDirect3D
    {
        #region Init, Reset and Dispose Methods

        /// <summary>
        /// 
        /// </summary>
        public D3D()
        {
            OnInteractiveInit();
        }

        /// <summary>
        /// 
        /// </summary>
        ~D3D() { Dispose(false); }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        { 
        }

        /// <summary>
        /// Size set with call to <see cref="Reset(DrawEventArgs)"/>
        /// </summary>
        public Vector2 RenderSize { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public virtual void Reset(DrawEventArgs args)
        {
            int w = (int)Math.Ceiling(args.RenderSize.Width);
            int h = (int)Math.Ceiling(args.RenderSize.Height);
            if (w < 1 || h < 1)
                return;

            RenderSize = new Vector2(w, h);
            if (Camera != null)
                Camera.AspectRatio = (float)(args.RenderSize.Width / args.RenderSize.Height);

            Reset(w, h);
            if (Resetted != null)
                Resetted(this, args);

            Render(args);

            if (args.Target != null)
                SetBackBuffer(args.Target);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public virtual void Reset(int w, int h)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DrawEventArgs> Resetted;


        #endregion
        
        #region Rendering Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract System.Windows.Media.Imaging.WriteableBitmap ToImage();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dximage"></param>
        public abstract void SetBackBuffer(D3D9ImageSource dximage);

        /// <summary>
        /// Time in the last <see cref="DrawEventArgs"/> passed to <see cref="Render(DrawEventArgs)"/>
        /// </summary>
        public TimeSpan RenderTime { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void Render(DrawEventArgs args)
        {
            RenderTime = args.TotalTime;
            if (Camera != null)
                Camera.FrameMove(args.DeltaTime);

            BeginRender(args);
            RenderScene(args);
            EndRender(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public virtual void BeginRender(DrawEventArgs args) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public virtual void RenderScene(DrawEventArgs args)
        {
            if (Rendering != null)
                Rendering(this, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public virtual void EndRender(DrawEventArgs args) { }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DrawEventArgs> Rendering; 
        
        #endregion

        #region Interaction Methods

        /// <summary>
        /// 
        /// </summary>
        private void OnInteractiveInit()
        {
        }

        /// <summary>
        /// Override this to focus the view, capture the mouse and select the <see cref="Camera"/> 
        /// </summary>
        public virtual void OnMouseDown(UIElement ui, MouseButtonEventArgs e)
        {
            if (Camera != null)
            {
                ui.CaptureMouse();
                ui.Focus();
                Camera.HandleMouseDown(ui, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnMouseMove(UIElement ui, MouseEventArgs e)
        {
            if (Camera != null && ui.IsMouseCaptured)
            {
                Camera.HandleMouseMove(ui, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnMouseUp(UIElement ui, MouseButtonEventArgs e)
        {
            if (Camera != null && ui.IsMouseCaptured)
            {
                Camera.HandleMouseUp(ui, e);
            }
            ui.ReleaseMouseCapture();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnMouseWheel(UIElement ui, MouseWheelEventArgs e)
        {
            if (Camera != null)
            {
                Camera.HandleMouseWheel(ui, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnKeyDown(UIElement ui, KeyEventArgs e)
        {
            if (Camera != null)
            {
                Camera.HandleKeyDown(ui, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnKeyUp(UIElement ui, KeyEventArgs e)
        {
            if (Camera != null)
            {
                Camera.HandleKeyUp(ui, e);
            }
        }

        #endregion

        #region Camera

        /// <summary>
        /// 
        /// </summary>
        public BaseCamera Camera
        {            
            get => _mCamera;
	        set { if (value == _mCamera) return; _mCamera = value; OnPropertyChanged("Camera"); }
        }

        /// <summary>
        /// 
        /// </summary>
        private BaseCamera _mCamera;

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// 
        /// </summary>
        protected void OnPropertyChanged(string name)
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
