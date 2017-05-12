using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Specialized;
using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ResourceDictionary LocalizationResources;

        public const int MinimumLocalizationVersion = 21;

        internal static DateTime StartTime { get; private set; }

        static App()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            LocalizationResources = (ResourceDictionary)Application.LoadComponent(new Uri("/Smellyriver.TankInspector;component/LocalizationResources.xaml", UriKind.Relative));

            App.StartTime = DateTime.UtcNow;
        }


        public static int LocalizationVersion
        {
            get
            {
	            if (LocalizationResources.Contains("LocalizationVersion"))
                    return (int)LocalizationResources["LocalizationVersion"];
	            return -1;
            }
        }

        public static string GetLocalizedString(string key, string defaultValue = null)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            var l10NKey = "L10N_" + key;

            if (LocalizationResources.Contains(l10NKey))
                return LocalizationResources[l10NKey] as string;

            if (defaultValue != null)
                return defaultValue;
	        return $"missing:{key}";
        }

        public static Cursor LoadCursor(string name)
        {
	        if (Application.Current is App)
            {
                var path = $"resources/cursors/{name}.cur";
                using (var stream = File.OpenRead(path))
                {
                    return new Cursor(stream);
                }
            }
	        return Cursors.Arrow;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (App.LocalizationVersion < MinimumLocalizationVersion)
            {
                var message = App.GetLocalizedString("ObsoleteLocalization", "This language pack of US English is obsolete and incompatible. Please update it, or delete it to use a fallback language.");
                var title = App.GetLocalizedString("ObsoleteLocalizationTitle", "Obsolete Language Pack");
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            var argLog = SafeLog.GetLogger("Args");
            foreach (var arg in e.Args)
                argLog.Info(arg);

            // todo: remove e.Args[1] (login token)

            if (e.Args.Length >= 3 && Database.IsPathValid(e.Args[2]))
            {
                var pathes = new StringCollection();
                pathes.Add(e.Args[2]);
                ApplicationSettings.Default.GamePathes = pathes;
                ApplicationSettings.Default.Save();
            }

            var versionLog = SafeLog.GetLogger("Version");
            var x = ApplicationSettings.Default.GamePathes; //dummy

            //ApplicationSettings.Default.GamePathes[0] = @"E:\Game Archives\World_of_Tanks_CT";

            Program.InitializeUnhandledExceptionHandler();

            versionLog.Info(Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Program.LogHardwareInfo();
        }
    }
}
