using System.Collections.Generic;

namespace Smellyriver.TankInspector.UIComponents
{
	internal interface IDetailedDataRelatedComponent
    {
        DetailedDataRelatedComponentType ComponentType { get; }
        IEnumerable<string> ModificationDomains { get; }
    }
}
