using System.Windows.Controls;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : UserControl
    {

        internal ShellViewModel ViewModel
        {
            get => this.DataContext as ShellViewModel;
	        set => this.DataContext = value;
        }

        public ShellView()
        {
            InitializeComponent();
        }
    }
}
