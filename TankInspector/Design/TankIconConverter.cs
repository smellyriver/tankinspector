using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Modeling;
using Smellyriver.TankInspector.UIComponents;
using System;
using System.Windows.Media.Imaging;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Design
{
	internal class TankIconConverter : DatabaseRelatedValueConverter<object, BitmapSource>
    {      

        public virtual TankIconType Type { get; set; }

        protected string GetIconKey(object tankObject)
        {
            if (tankObject == null)
                return null;

            var vm = tankObject as TankViewModelBase;
            if (vm != null)
                return vm.IconKey;

            var tank = tankObject as Tank;
            if (tank != null)
                return tank.IconKey;

            return (string)tankObject.GetPropertyValue("IconKey");
        }

        protected override BitmapSource Convert(object tank, string databaseRootPath)
        {
            try
            {
                if (tank == null)
                    return null;

                return TankIcon.Load(databaseRootPath, this.GetIconKey(tank), 96, this.Type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
