using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for CreditsView.xaml
    /// </summary>
    public partial class CreditsView : UserControl
    {

        internal CreditsViewModel ViewModel
        {
            get => this.DataContext as CreditsViewModel;
	        set => this.DataContext = value;
        }

        public CreditsView()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(this.AdditionalLocalizationCredits.Text))
                this.AdditionalLocalizationCreditsContainer.Visibility = Visibility.Collapsed;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ViewModel.Hide();
        }

        private void SpecialThankLink_Click(object sender, RoutedEventArgs e)
        {
            var specialThank = (CreditsViewModel.SpecialThankItem)((Button)sender).DataContext;
	        Process.Start(specialThank.Url);
        }

        private void WebsiteLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/smellyriver/tankinspector");
        }

        private void LocalizerLink_Click(object sender, RoutedEventArgs e)
        {
            var url = App.GetLocalizedString("LocalizerLinkURL");
            if (url == "(empty)" || string.IsNullOrEmpty(url))
                return;
            Process.Start(url);
        }

    }
}
