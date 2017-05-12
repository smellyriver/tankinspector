using System;
using Smellyriver.TankInspector.Graphics.Frameworks;

namespace Smellyriver.TankInspector.Graphics.Scene
{
    public class SceneMeshBase : IDirect3D, IDisposable
    {
        public virtual void Reset(DrawEventArgs args)
        {
        }

        public virtual void Render(DrawEventArgs args)
        {
        }


        ~SceneMeshBase() { Dispose(false); }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
