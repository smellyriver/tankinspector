using Smellyriver.TankInspector.Modeling;
using Smellyriver.Wpf.Input;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class SearchResultItemViewModel
    {


        public ITankInfo Tank { get; }

        public HangarViewModel Owner { get; }
        private CommandBindingCollection _commandBindings;

        public ICommand SelectTankCommand { get; }
        public ICommand SelectAsReferenceTankCommand { get; }
        public SearchResultItemViewModel(CommandBindingCollection commandBindings, ITankInfo tankInfo, HangarViewModel owner)
        {
            _commandBindings = commandBindings;
            this.Tank = tankInfo;
            this.Owner = owner;

            this.SelectTankCommand = new RelayCommand(() => owner.SelectSearchResultTank(this.Tank));
            this.SelectAsReferenceTankCommand = new RelayCommand(() => owner.SelectSearchResultTankAsReferenceTank(this.Tank));
        }

    }
}
