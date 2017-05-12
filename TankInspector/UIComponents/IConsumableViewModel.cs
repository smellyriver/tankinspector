using System.Collections.Generic;

namespace Smellyriver.TankInspector.UIComponents
{
	internal interface IConsumableViewModel : IAccessoryViewModel
    {
        IEnumerable<string> Tags { get; }
        IEnumerable<string> IncompatibleTags { get; }
    }
}
