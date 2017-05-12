using Smellyriver.TankInspector.Modeling;
using System.Collections.Generic;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class NationalTechTreeNodeViewModel : TankGalleryItemViewModel
    {

        public int Column { get; }
        public int Row { get; }
        public IEnumerable<string> UnlockTanks { get; }

        internal NationalTechTreeNodeViewModel(CommandBindingCollection commandBindings, Tank tank, int row, int column, IEnumerable<string> unlockeTanks)
            : base(commandBindings, tank)
        {
            this.Column = column;
            this.Row = row;
            this.UnlockTanks = unlockeTanks;
        }

        internal NationalTechTreeNodeViewModel(CommandBindingCollection commandBindings, Tank tank, int row, int column)
            : this(commandBindings, tank, row, column, new string[0])
        {

        }
    }
}
