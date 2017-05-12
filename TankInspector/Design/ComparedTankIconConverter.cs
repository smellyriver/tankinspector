using Smellyriver.TankInspector.IO;
using System;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class ComparedTankIconConverter : TankIconConverter
    {
        public static readonly string AverageTankKey = "__AVERAGE";

        public override TankIconType Type
        {
            get => TankIconType.Contour;
	        set => throw new NotSupportedException();
        }

        protected override BitmapSource Convert(object tank, string databaseRootPath)
        {
            if(tank == null)
                return base.Convert(tank, databaseRootPath);

            if (this.GetIconKey(tank) == AverageTankKey)
                return new BitmapImage(new Uri("/Resources/Images/UIElements/AverageTankIcon.png", UriKind.Relative));

            return base.Convert(tank, databaseRootPath);
        }
    }
}
