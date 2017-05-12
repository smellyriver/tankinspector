using Smellyriver.TankInspector.Modeling;
using System;
using System.Windows.Data;

namespace Smellyriver.TankInspector.Design
{
	internal abstract class DatabaseRelatedValueConverter<TArg, TResult> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var databaseRootPath = parameter as string;
            if (string.IsNullOrEmpty(databaseRootPath))
            {
                var databaseObject = value as IDatabaseObject;
                if (databaseObject != null)
                    databaseRootPath = databaseObject.Database.RootPath;

                if (string.IsNullOrEmpty(databaseRootPath))
                    databaseRootPath = Database.Current.RootPath;
            }

            return this.Convert((TArg)value, databaseRootPath);
        }

        protected abstract TResult Convert(TArg value, string databaseRootPath);

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
