using System;

namespace Smellyriver.TankInspector.UIComponents
{
	internal interface IReferenceTankProvider
    {
        event EventHandler<ReferenceTankChangedEventArgs> ReferenceTankChanged;

        TankViewModelBase ReferenceTank { get; }
    }
}
