using Smellyriver.Wpf.Input;
using System;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal abstract class MainWindowOverlayViewModel : NotificationObject
    {

        private bool _isShown;
        public bool IsShown
        {
            get => _isShown;
	        private set
            {
                _isShown = value;
                this.RaisePropertyChanged(() => this.IsShown);

                if (this.VisibilityChanged != null)
                    this.VisibilityChanged(this, new EventArgs());
            }
        }

        public abstract bool IsFullWindow { get; }

        public virtual void Show()
        {
            this.IsShown = true;
        }

        public virtual void Hide()
        {
            this.IsShown = false;
        }

        public void Toggle()
        {
            if (this.IsShown)
                this.Hide();
            else
                this.Show();
        }

        protected MainWindowViewModel Owner { get; }

        public event EventHandler VisibilityChanged;

        public ICommand ShowCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand ToggleCommand { get; }

        public MainWindowOverlayViewModel(MainWindowViewModel owner)
        {
            Owner = owner;

            this.ShowCommand = new RelayCommand(this.Show);
            this.HideCommand = new RelayCommand(this.Hide);
            this.ToggleCommand = new RelayCommand(this.Toggle);
        }

    }
}
