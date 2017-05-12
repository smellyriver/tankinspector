using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal abstract class DatabaseRelatedMultiValueConverter<TResult> : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var databaseRootPath = parameter as string;
            if (string.IsNullOrEmpty(databaseRootPath))
            {
                if (values.Length > 0)
                {
                    var databaseObject = values[0] as IDatabaseObject;
                    if (databaseObject != null)
                        databaseRootPath = databaseObject.Database.RootPath;
                }

                if (string.IsNullOrEmpty(databaseRootPath))
                    databaseRootPath = Database.Current.RootPath;
            }

            return this.Convert(values, databaseRootPath);
        }

        protected abstract TResult Convert(object[] values, string databaseRootPath);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
