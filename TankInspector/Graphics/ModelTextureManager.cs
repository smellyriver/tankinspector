using SharpDX.Direct3D9;
using Smellyriver.TankInspector.IO;
using System.Collections.Generic;
using System.IO;

namespace Smellyriver.TankInspector.Graphics
{
	internal class ModelTextureManager
    {
	    private readonly Dictionary<string, ModelTexture> _textures = new Dictionary<string, ModelTexture>();

        public ModelTexture LoadTexture(IPackageIndexer packageIndexer,string texturePath,Device device)
        {
            using (var stream = ModelTextureManager.OpenTexture(packageIndexer, texturePath))
            {
                var pos = stream.Position;  

                lock(_textures)
                {
					if (!_textures.TryGetValue(texturePath, out ModelTexture texture))
					{
						var info = ImageInformation.FromStream(stream);
						stream.Seek(pos, SeekOrigin.Begin);
						texture = new ModelTexture(texturePath, Texture.FromStream(device, stream), info);
						_textures.Add(texturePath, texture);
					}
					else
					{
						texture.AddRef();
					}
					return texture;
                }
            }
        }

        public void UnloadTexture(ModelTexture texture)
        {
            lock (_textures)
            {
                texture.RemoveRef();
                if (texture.RefCount == 0)
                {
                    _textures.Remove(texture.TexturePath);
                }
            }
        }

        private static PackageStream OpenTexture(IPackageIndexer packageIndexer, string path)
        {
            var packagePath = packageIndexer.GetPackagePath(path);

            if (packagePath == null && Path.GetExtension(path) == ".tga")
            {
                path = path.Substring(0, path.Length - 4) + ".dds";
                packagePath = packageIndexer.GetPackagePath(path);
            }

            return new PackageStream(packagePath, path);
        }


    }
}
