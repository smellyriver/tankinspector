using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for MessagesWindow.xaml
    /// </summary>
    public partial class MessagesWindow : Window
    {
        internal MessagesWindowViewModel ViewModel
        {
            get => this.DataContext as MessagesWindowViewModel;
	        set => this.DataContext = value;
        }

        internal MessagesWindow(MessagesWindowViewModel viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MessageHyperlink_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }
    }
}
