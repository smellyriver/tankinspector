using System.Collections.Generic;
using System.IO;
using System.Xml;
using Smellyriver.TankInspector.IO.XmlDecoding;
using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.Graphics
{
    public class ModelVisual : ModelSectionDeserializable
    {
        public List<ModelRenderSet> RenderSets { get; set; }

        protected ModelVisual()
        {
            RenderSets = new List<ModelRenderSet>();
        }


        public override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "renderSet":
                    ModelRenderSet renderSet;
                    reader.Read(out renderSet);
                    renderSet.Visual = this;
                    RenderSets.Add(renderSet);
                    return true;

                default:
                    return false;
            }
        }

        public static ModelVisual ReadFrom(Stream visualStream)
        {
            var visual = new ModelVisual();

            using (var visualReader = new BigworldXmlReader(visualStream))
            {
                visualReader.ReadStartElement();
                visual.Deserialize(visualReader);
                visualReader.ReadEndElement();
            }

            return visual;
        }
    }
}
