using Smellyriver.TankInspector.Modeling;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        internal SettingsViewModel ViewModel
        {
            get => this.DataContext as SettingsViewModel;
	        set => this.DataContext = value;
        }


        private void BrowseGameFolderButton_Click(object sender, RoutedEventArgs e)
        {
            GamePathTextBox.Text = Database.ShowSelectGameRootFolderDialog(GamePathTextBox.Text);
            GamePathTextBox.Focus();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Close()
        {
            if (this.CheckGamePath())
            {
                this.ViewModel.Hide();
                if (this.ViewModel.IsGameFolderChanged)
                {
                    MessageBox.Show(App.GetLocalizedString("GameFolderChangedMessage"), App.GetLocalizedString("GameFolderChangedTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                    this.ViewModel.IsGameFolderChanged = false;
                }
            }
        }


        private bool CheckGamePath()
        {
            if (!Database.IsPathValid(this.GamePathTextBox.Text))
            {
                MessageBox.Show(App.GetLocalizedString("SelectedGamePathIsInvalid"), App.GetLocalizedString("InvalidGamePath"),
                                MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        private void GamePathTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CheckGamePath();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        private void GamePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.GamePathTextBox.Text) && Database.IsPathValid(this.GamePathTextBox.Text))
                this.ViewModel.GameFolder = this.GamePathTextBox.Text;
        }
    }
}
