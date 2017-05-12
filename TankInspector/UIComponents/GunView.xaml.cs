using System.Windows.Controls;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for GunView.xaml
    /// </summary>
    public partial class GunView : UserControl
    {

        internal GunViewModel ViewModel
        {
            get => this.DataContext as GunViewModel;
	        set => this.DataContext = value;
        }

        public GunView()
        {
            InitializeComponent();
        }
    }
}
