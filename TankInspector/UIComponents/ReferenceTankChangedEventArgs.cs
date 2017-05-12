using System;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class ReferenceTankChangedEventArgs : EventArgs
    {
        public TankViewModelBase Tank { get; }

        public ReferenceTankChangedEventArgs(TankViewModelBase tank)
        {
            this.Tank = tank;
        }

    }
}
