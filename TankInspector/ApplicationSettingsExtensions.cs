using System.Collections.Specialized;
using System.Linq;

namespace Smellyriver.TankInspector
{
	internal static class ApplicationSettingsExtensions
    {
        public static void EnsureGamePathesDistinct(this ApplicationSettings settings)
        {
            var pathes = settings.GamePathes;
            settings.GamePathes = new StringCollection();

            foreach (var path in pathes.Cast<string>().Distinct())
            {
                settings.GamePathes.Add(path);
            }
        }
    }
}
