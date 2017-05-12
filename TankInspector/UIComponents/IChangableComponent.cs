using System.Collections;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal interface IChangableComponent : IPreviewable
    {
        ICommand EquipCommand { get; }
        bool IsElite { get; }
        bool IsEquipped { get; }
        
        IEnumerable Replacements { get; }
    }
}
