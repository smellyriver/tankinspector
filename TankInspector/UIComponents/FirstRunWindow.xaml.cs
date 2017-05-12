using Microsoft.Win32;
using Smellyriver.TankInspector.Modeling;
using System.Windows;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for FirstRunWindow.xaml
    /// </summary>
    public partial class FirstRunWindow : Window
    {
        public FirstRunWindow()
        {
            InitializeComponent();

            var wotReplayIcon = Registry.GetValue(@"HKEY_CLASSES_ROOT\.wotreplay\shell\open\command", "", null) as string;
            if (!string.IsNullOrEmpty(wotReplayIcon) && wotReplayIcon.StartsWith("\""))
            {
                var closingQuote = wotReplayIcon.IndexOf('"', 1);
                if (closingQuote >= 0)
                {
                    var lastSlash = wotReplayIcon.LastIndexOfAny(new[] { '\\', '/' }, closingQuote);
                    if (lastSlash >= 0)
                    {
                        var path = wotReplayIcon.Substring(1, lastSlash);

                        if (Database.IsPathValid(path))
                            this.GamePathTextBox.Text = path;
                    }
                }
            }
        }

        private void BrowseGameFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var path = Database.ShowSelectGameRootFolderDialog(this.GamePathTextBox.Text);
            if (!string.IsNullOrEmpty(path))
                this.GamePathTextBox.Text = path;
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            var path = System.IO.Path.GetFullPath(this.GamePathTextBox.Text);
            if (!Database.IsPathValid(path))
            {
                MessageBox.Show(this, App.GetLocalizedString("SelectedGamePathIsInvalid"), App.GetLocalizedString("InvalidGamePath"),
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ApplicationSettings.Default.GamePathes.Add(path);
            ApplicationSettings.Default.EnsureGamePathesDistinct();

            ApplicationSettings.Default.Save();

            this.DialogResult = true;
            this.Close();
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
