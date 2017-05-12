using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Data;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Design
{
	internal class GuiImageConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = null;

            var propertyName = parameter as string;

            if (string.IsNullOrEmpty(propertyName))
                path = value as string;
            else
            {
				if (value.TryGetPropertyValue(propertyName, out object pathObject))
					path = pathObject as string;
			}

            if (string.IsNullOrEmpty(path))
                return null;

            string databaseRootPath = null;

            var databaseObject = value as IDatabaseObject;
            if (databaseObject != null)
                databaseRootPath = databaseObject.Database.RootPath;

            if (string.IsNullOrEmpty(databaseRootPath))
                databaseRootPath = Database.Current.RootPath;


            try
            {
                return PackageImage.Load(databaseRootPath, PackageImage.GuiPackage, path);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}