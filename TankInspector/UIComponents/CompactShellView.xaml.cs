using System.Windows.Controls;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for CompactShellView.xaml
    /// </summary>
    public partial class CompactShellView : UserControl
    {
        internal ShellViewModel ViewModel
        {
            get => this.DataContext as ShellViewModel;
	        set => this.DataContext = value;
        }

        public CompactShellView()
        {
            InitializeComponent();
        }
    }
}
