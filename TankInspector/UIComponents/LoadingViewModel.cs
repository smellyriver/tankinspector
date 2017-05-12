using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class LoadingViewModel : NotificationObject
    {

        public bool IsFirstTimeLoading { get; }

        private bool _hideLoadingViewCompleted;
        public bool HideLoadingViewCompleted
        {
            get => _hideLoadingViewCompleted;
	        set
            {
                _hideLoadingViewCompleted = value;
                if (_hideLoadingViewCompleted)
                    _owner.UnloadLoadingView();
            }
        }

        private bool _loadingCompleted;
        public bool LoadingCompleted
        {
            get => _loadingCompleted;
	        set
            {
                _loadingCompleted = value;
                this.RaisePropertyChanged(() => this.LoadingCompleted);
            }
        }

        private readonly MainWindowViewModel _owner;

	    public LoadingViewModel(CommandBindingCollection commandBindings, MainWindowViewModel owner)
        {
	        _owner = owner;

            this.IsFirstTimeLoading = AppState.Default.FirstRun;
        }
    }
}
