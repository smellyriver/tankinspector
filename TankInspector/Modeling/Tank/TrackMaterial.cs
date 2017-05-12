using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class TrackMaterial : LodVisual
    {

        private string _leftMaterial;
        public string LeftMaterial => _leftMaterial;

	    private string _rightMaterial;
        public string RightMaterial => _rightMaterial;

	    private double _textureScale;
        public double TextureScale => _textureScale;


	    protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "leftMaterial":
                    reader.Read(out _leftMaterial);
                    return true;
                case "rightMaterial":
                    reader.Read(out _rightMaterial);
                    return true;
                case "textureScale":
                    reader.Read(out _textureScale);
                    return true;
                default:
                    return base.DeserializeSection(name, reader);
            }
        }
    }
}
