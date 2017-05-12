using System;
using Smellyriver.TankInspector.Graphics.Frameworks;
using Smellyriver.Wpf;
using System.ComponentModel;

namespace Smellyriver.TankInspector.Graphics.Scene
{
    public abstract class SceneBase<T> : DependencyNotificationObject , IDirect3D , IDisposable
		where T : D3D
	{
		/// <summary>
		/// 
		/// </summary>
		private T _mContext;

		/// <summary>
		/// 
		/// </summary>
		public virtual T Renderer 
		{
			get { return _mContext; }
			set
			{
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Renderer != null)
                    {
                        Renderer.Rendering -= ContextRendering;
                        Detach();
                    }
                    _mContext = value;
                    if (Renderer != null)
                    {
                        Renderer.Rendering += ContextRendering;
                        Attach();
                    }
                }
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public abstract void RenderScene(DrawEventArgs args);

		/// <summary>
		/// 
		/// </summary>
        public virtual void Dispose()
        {
            Disposer.RemoveAndDispose(ref _mContext);
        }

		/// <summary>
		/// 
		/// </summary>
		protected abstract void Attach();

		/// <summary>
		/// 
		/// </summary>
		protected abstract void Detach();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		void IDirect3D.Reset(DrawEventArgs args)
		{
			if (Renderer != null)
				Renderer.Reset(args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		void IDirect3D.Render(DrawEventArgs args)
		{
			if (Renderer != null)
				Renderer.Render(args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aCtx"></param>
		/// <param name="args"></param>
		private void ContextRendering(object aCtx, DrawEventArgs args) { RenderScene(args); }


	}
}
