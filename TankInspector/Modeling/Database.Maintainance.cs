using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Smellyriver.TankInspector.Modeling
{
	internal partial class Database
    {
        private static Dictionary<DatabaseKey, Database> _sLoadedDatabases;

        private static Database _current;
        public static Database Current
        {
            get => _current;
	        set
            {
                _current = value;
                if (Database.CurrentDatabaseChanged != null)
                    Database.CurrentDatabaseChanged(null, null);
            }
        }

        public static event EventHandler CurrentDatabaseChanged;

        public static DatabaseKey? CurrentDatabaseKey
        {
            get => Database.Current == null ? (DatabaseKey?)null : Database.Current.Key;
	        set
            {
                if (value == null)
                {
                    Database.Current = null;
                    ApplicationSettings.Default.PreviousDatabaseKeyHash = null;
                }
                else
                {
                    ApplicationSettings.Default.PreviousDatabaseKeyHash = value.Value.HashString;
                }
                ApplicationSettings.Default.Save();
            }
        }

        public static DatabaseKey? LoadFromPath(string path)
        {
            var database = new Database(path);

            if (_sLoadedDatabases.ContainsKey(database.Key))
            {
                Log.WarnFormat("attempting to load an existed game client: version='{0}', path='{1}'", database.Key.Version, database.Key.RootPath);

                MessageBox.Show(Application.Current.MainWindow, App.GetLocalizedString("GameVersionExisted"), App.GetLocalizedString("GameVersionExistedTitle"),
                            MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
	        _sLoadedDatabases.Add(database.Key, database);

	        Log.InfoFormat("game client added to database list: version='{0}', path='{1}'", database.Key.Version, database.Key.RootPath);

	        ApplicationSettings.Default.GamePathes.Add(path);
	        ApplicationSettings.Default.Save();

	        return database.Key;
        }

        public static DatabaseKey? PromptAddDatabase()
        {
            var path = Database.ShowSelectGameRootFolderDialog();
            if (path == null)
                return null;

            return Database.LoadFromPath(path);
        }

        internal static string ShowSelectGameRootFolderDialog(string path = null)
        {
            while (true)
            {
                var dialog = new VistaFolderBrowserDialog();
                dialog.SelectedPath = path;
                dialog.Description = App.GetLocalizedString("SelectGameRootFolder");
                dialog.ShowNewFolderButton = false;
                if (dialog.ShowDialog(Application.Current.MainWindow).Value)
                {
	                if (Database.IsPathValid(dialog.SelectedPath))
                        return dialog.SelectedPath;
	                MessageBox.Show(Application.Current.MainWindow, App.GetLocalizedString("SelectedGamePathIsInvalid"), App.GetLocalizedString("InvalidGamePath"),
		                MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    return path;
            }
        }

        public static bool ValidateGamePathes()
        {
            _sLoadedDatabases = new Dictionary<DatabaseKey, Database>();

            var invalidPathes = new List<string>();
            var validPathes = new List<string>();

            foreach (var path in ApplicationSettings.Default.GamePathes)
            {
                if (!Database.IsPathValid(path))
                    invalidPathes.Add(path);
                else
                    validPathes.Add(path);
            }

            if (invalidPathes.Count > 0)
                Database.AlertInvalidPathes(invalidPathes);

            while (validPathes.Count == 0)
            {
                var path = Database.ShowSelectGameRootFolderDialog();
                if (path != null)
                {
                    validPathes.Add(path);
                    break;
                }
	            return false;
            }

            ApplicationSettings.Default.GamePathes.Clear();
            ApplicationSettings.Default.GamePathes.AddRange(validPathes.ToArray());
            ApplicationSettings.Default.Save();

            return true;
        }

        public static void Load(bool fullyLoad, DatabaseKey? currentDatabaseKey = null)
        {
            _sLoadedDatabases = new Dictionary<DatabaseKey, Database>();

            var paths = ApplicationSettings.Default.GamePathes.OfType<string>().ToArray();

            foreach (var path in paths)
            {
                Log.InfoFormat("loading database from ApplicationSettings: path='{0}', fullyLoad='{1}'", path, fullyLoad);

                var database = new Database(path);

                _sLoadedDatabases.Add(database.Key, database);

                if (fullyLoad)
                    database.FullyLoad();
            }


			if (currentDatabaseKey == null && ApplicationSettings.Default.PreviousDatabaseKeyHash != null)
				currentDatabaseKey = _sLoadedDatabases.Keys.FirstOrDefault(k => k.HashString == ApplicationSettings.Default.PreviousDatabaseKeyHash);

			if (currentDatabaseKey == null || !_sLoadedDatabases.TryGetValue(currentDatabaseKey.Value, out Database currentDatabase))
            {
                Log.Info("no database matching a previous saved database key found, gonna pick the last database in the database list as current");
                currentDatabase = _sLoadedDatabases.Values.Last();
            }

            if(currentDatabase == null)
                Log.Fatal("no database could be loaded as current database");

            currentDatabase.FullyLoad();
            Database.Current = currentDatabase;
        }

        private static void AlertInvalidPathes(IEnumerable<string> pathes)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(App.GetLocalizedString("FollowingAreInvalidGamePathes"));
            foreach (var path in pathes)
                messageBuilder.Append("    ").AppendLine(path);

            MessageBox.Show(Application.Current.MainWindow, messageBuilder.ToString(), App.GetLocalizedString("InvalidGamePath"), MessageBoxButton.OK, MessageBoxImage.Exclamation);

        }

        public static IEnumerable<DatabaseKey> AvailableDatabaseKeys => _sLoadedDatabases == null ? (IEnumerable<DatabaseKey>)new DatabaseKey[0] : _sLoadedDatabases.Keys;

	    public static Database GetDatabase(DatabaseKey key)
        {
            var database = _sLoadedDatabases[key];

            if(database == null)
                Log.FatalFormat("trying to get a non-existed database: version='{0}', path='{1}'", key.Version, key.RootPath);

            if (!database.IsFullyLoaded)
                database.FullyLoad();

            return database;
        }

    }
}
