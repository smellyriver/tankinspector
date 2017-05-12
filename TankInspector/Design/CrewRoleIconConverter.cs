using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class CrewRoleIconConverter : DatabaseRelatedValueConverter<CrewRoleType?, BitmapSource>
    {
        public virtual CrewRoleIconType Type { get; set; }

        protected override BitmapSource Convert(CrewRoleType? roleType, string databaseRootPath)
        {
            try
            {
                if (roleType == null)
                    return null;

                string iconKey;
                switch (roleType.Value)
                {
                    case CrewRoleType.Commander:
                        iconKey = "commander";
                        break;
                    case CrewRoleType.Driver:
                        iconKey = "driver";
                        break;
                    case CrewRoleType.Gunner:
                        iconKey = "gunner";
                        break;
                    case CrewRoleType.Loader:
                        iconKey = "loader";
                        break;
                    case CrewRoleType.Radioman:
                        iconKey = "radioman";
                        break;
                    default:
                        throw new NotSupportedException();
                }

                string type;
                switch (this.Type)
                {
                    case CrewRoleIconType.Big:
                        type = "big";
                        break;
                    case CrewRoleIconType.Small:
                        type = "small";
                        break;
                    default:
                        throw new NotSupportedException();
                }

                var path = $"gui/maps/icons/tankmen/roles/{type}/{iconKey}.png";

                return PackageImage.Load(databaseRootPath, PackageImage.GuiPackage, path);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
