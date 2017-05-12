using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.DataAnalysis
{
	internal interface IValueTipContentProvider
    {
        IEnumerable<Inline> ValueTipContent { get; }
        double MaxHeight { get; }
    }
}
