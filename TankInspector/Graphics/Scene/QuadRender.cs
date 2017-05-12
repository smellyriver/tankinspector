using System.Reflection;
using log4net;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Graphics.Frameworks;

namespace Smellyriver.TankInspector.Graphics.Scene
{
	internal class QuadRender : SceneMeshBase
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private VertexBuffer _vertices;
        private IndexBuffer _indices;
        private VertexDeclaration _vertexDeclaration;

        protected override void Dispose(bool disposing)
        {
            Log.Info("dispose");
            if (disposing)
            {
                Disposer.RemoveAndDispose(ref _vertices);
                Disposer.RemoveAndDispose(ref _indices);
                Disposer.RemoveAndDispose(ref _vertexDeclaration);
            }
        }

        public QuadRender(Device device,int width,int height)
        {
            Log.Info("new");
            _vertices = new VertexBuffer(device, (SharpDX.Utilities.SizeOf<Vector3>() + SharpDX.Utilities.SizeOf<Vector2>()) * 4, Usage.WriteOnly, VertexFormat.None, Pool.Default);

            Vector2 pixelSize = new Vector2(1.0f / (float)width, 1.0f / (float)height);

            //Vector2 pixelSize = new Vector2(0,0);

            var data = _vertices.Lock(0, 0, LockFlags.None);

            data.Write(new Vector3( -1.0f - pixelSize.X,  1.0f + pixelSize.Y, 1.0f));
            data.Write(new Vector2( 0.0f, 0.0f));
            data.Write(new Vector3( 1.0f - pixelSize.X, 1.0f + pixelSize.Y, 1.0f));
            data.Write(new Vector2( 1.0f, 0.0f));
            data.Write(new Vector3( -1.0f - pixelSize.X, -1.0f + pixelSize.Y, 1.0f));
            data.Write(new Vector2( 0.0f, 1.0f));
            data.Write(new Vector3(1.0f - pixelSize.X, -1.0f + pixelSize.Y, 1.0f));
            data.Write(new Vector2( 1.0f, 1.0f));

            _vertices.Unlock();
            _indices = new IndexBuffer(device, sizeof(short) * 6, Usage.WriteOnly, Pool.Default, true);

            _indices.Lock(0, 0, LockFlags.None).WriteRange(new short[]
            {
                0, 1, 2, 1, 3, 2
            });
            _indices.Unlock();

            _vertexDeclaration = new VertexDeclaration(device, new[] { 
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
            });
        }


        internal void Render(Device device)
        {
            device.SetStreamSource(0, _vertices, 0, (SharpDX.Utilities.SizeOf<Vector3>() + SharpDX.Utilities.SizeOf<Vector2>()));
            device.VertexDeclaration = _vertexDeclaration;
            device.Indices = _indices;
            device.DrawIndexedPrimitive(PrimitiveType.TriangleList,0,0,4,0,2);
        }
    }
}
