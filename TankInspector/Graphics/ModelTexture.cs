using SharpDX.Direct3D9;

namespace Smellyriver.TankInspector.Graphics
{
	internal class ModelTexture
    {
        public ImageInformation ImageInformation { get; }

        public ModelTexture(string texturePath, Texture texture)
        {
            TexturePath = texturePath;
            Texture = texture;
            RefCount = 1;
        }

        public ModelTexture(string texturePath, Texture texture, ImageInformation info) : this(texturePath, texture)
        {
            this.ImageInformation = info;
        }

        public string TexturePath { get; }
        public Texture Texture { get; }
        public int RefCount { get; private set; }

        public void AddRef()
        {
            ++RefCount;
        }

        public void RemoveRef()
        {
            --RefCount;
            if(RefCount==0)
            {
                Texture.Dispose();
            }
        }
    }
}
