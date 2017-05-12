using Smellyriver.TankInspector.Modeling;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class TankGalleryPageViewModel : NotificationObject
    {
        private readonly TankGalleryViewModel _owner;
        private CommandBindingCollection _commandBindings;

        public TankGalleryPageViewModel(CommandBindingCollection commandBindings, TankGalleryViewModel owner)
        {
            _commandBindings = commandBindings;
            _owner = owner;
        }

        internal void LoadTank(Tank tank)
        {
            _owner.LoadTank(tank);
        }
    }
}
