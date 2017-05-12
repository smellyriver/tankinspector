using System.Windows.Controls;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for TankGalleryView.xaml
    /// </summary>
    public partial class TankGalleryView : UserControl
    {

        internal TankGalleryViewModel ViewModel
        {
            get => this.DataContext as TankGalleryViewModel;
	        set => this.DataContext = value;
        }

        public TankGalleryView()
        {
            InitializeComponent();
        }

    }
}
