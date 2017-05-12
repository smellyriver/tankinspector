using System.Windows.Controls;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for LoadingView.xaml
    /// </summary>
    public partial class LoadingView : UserControl
    {

        internal LoadingViewModel ViewModel
        {
            get => this.DataContext as LoadingViewModel;
	        set => this.DataContext = value;
        }

        public LoadingView()
        {
            InitializeComponent();
        }

    }
}
