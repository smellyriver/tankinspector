using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Design
{
	internal class ShellIconConverter : DatabaseRelatedValueConverter<Shell, BitmapSource>
    {
        protected override BitmapSource Convert(Shell shell, string databaseRootPath)
        {
            try
            {
                if (shell == null)
                    return null;

                return ShellIcon.Load(databaseRootPath, shell.Type, 96);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
