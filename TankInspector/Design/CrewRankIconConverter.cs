using Smellyriver.TankInspector.IO;
using System;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class CrewRankIconConverter : DatabaseRelatedMultiValueConverter<BitmapSource>
    {
        public virtual CrewRankIconType Type { get; set; }

        protected override BitmapSource Convert(object[] values, string databaseRootPath)
        {
            try
            {
                if (values == null)
                    return null;

                var nationKey = (string)values[0];
                var rankLevel = (int)values[1];

                if (nationKey != "china")
                    rankLevel = 12 - rankLevel;

                string type;
                switch (this.Type)
                {
                    case CrewRankIconType.Big:
                        type = "big";
                        break;
                    case CrewRankIconType.Small:
                        type = "small";
                        break;
                    default:
                        throw new NotSupportedException();
                }

                var path = $"gui/maps/icons/tankmen/ranks/{type}/{nationKey}-{rankLevel}.png";

                return PackageImage.Load(databaseRootPath, PackageImage.GuiPackage, path);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
