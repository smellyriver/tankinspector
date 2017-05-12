using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class TankGalleryItemViewModel : NotificationObject
    {

        // use Tank instead of TankViewModel here because TankViewModel is too heavy to build,
        // while Tank-s are already existed in memory
        public Tank Tank { get; }

        protected CommandBindingCollection CommandBindings { get; }

        public TankGalleryItemViewModel(CommandBindingCollection commandBindings, Tank tank)
        {
            this.CommandBindings = commandBindings;
            this.Tank = tank;
        }

    }
}
